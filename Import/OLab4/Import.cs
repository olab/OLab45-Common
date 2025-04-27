using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using OLab.Access.Interfaces;
using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Utils;
using OLab.Data;
using OLab.Import.Interface;
using OLab.Import.OLab3.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Import.OLab4;

public partial class Importer : IImporter
{
  // Folder where the xtracted import file resides
  private string ExtractFolderName;
  private ScopedObjects _scopedObjectPhys;
  private Maps _newMapPhys;

  /// <summary>
  /// Run the import process on the data in the stream
  /// </summary>
  /// <param name="stream">Stream with improt archive file</param>
  /// <param name="fileName">File name of improt file</param>
  /// <param name="token"></param>
  /// <returns></returns>
  public async Task<Maps> Import(
    IOLabAuthorization auth,
    Stream stream,
    string fileName,
    CancellationToken token = default)
  {
    try
    {
      Authorization = auth;

      var transaction = _dbContext.Database.BeginTransaction();

      // reset message buffer so we just save the new messages
      GetLogger().Clear();

      var mapFullDto = await ExtractImportMapDefinition(
        stream,
        fileName,
        token );

      _scopedObjectPhys = new ScopedObjects(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider() );

      _newMapPhys = await WriteMapToDatabaseAsync( auth, mapFullDto, token );

      await ProcessMapNodesAsync( mapFullDto, token );
      await ProcessImportFilesAsync( token );

      await _scopedObjectPhys.WriteAllToDatabaseAsync( _newMapPhys.Id, token );

      await ProcessMapNodesScopedObjectsAsync( mapFullDto, token );

      await CleanupImportFilesAsync();

      if ( GetLogger().HasErrorMessage() )
        await _dbContext.Database.RollbackTransactionAsync();
      else
        await _dbContext.Database.CommitTransactionAsync();

      return _newMapPhys;

    }
    catch ( Exception ex )
    {
      GetLogger().LogError( $"Import error {ex.Message}" );
      await _dbContext.Database.RollbackTransactionAsync();
      throw;
    }
  }

  private async Task ProcessMapNodesScopedObjectsAsync(MapsFullRelationsDto mapFullDto, CancellationToken token)
  {
    // import the map nodes, save the new node ids for
    // when we import the map node links
    foreach ( var mapNodeDto in mapFullDto.MapNodes )
    {
      // remap 'true' MR's before Avatars since Avatars are rendered as MR's.
      RemapWikiTags<MediaResourceWikiTag>( mapNodeDto );
      RemapWikiTags<QuestionWikiTag>( mapNodeDto );
      RemapWikiTags<ConstantWikiTag>( mapNodeDto );
      RemapWikiTags<CounterWikiTag>( mapNodeDto );
      ReplaceVpdWikiTags( mapNodeDto );
      ReplaceAvWikiTags( mapNodeDto );
    }

  }

  /// <summary>
  /// Loads import import file into memory
  /// </summary>
  /// <param name="importFileName">ExportAsync ZIP file name</param>
  /// <returns>MapsFullRelationsDto</returns>
  private async Task<MapsFullRelationsDto> ExtractImportMapDefinition(
    Stream importFileStream,
    string importFileName,
    CancellationToken token = default)
  {
    ExtractFolderName = Path.GetFileNameWithoutExtension( importFileName );
    GetLogger().LogInformation( $"Folder extract directory: {ExtractFolderName}" );

    // delete any existing import files left over
    // from previous run
    await _fileModule.DeleteFolderAsync(
      _fileModule.BuildPath(
        OLabFileStorageModule.ImportRoot,
        ExtractFolderName ) );

    // save import file to storage
    await _fileModule.WriteFileAsync(
      importFileStream,
      _fileModule.BuildPath(
        OLabFileStorageModule.ImportRoot,
        importFileName ),
      token );

    // extract import file to storage
    await _fileModule.ExtractFileToStorageAsync(
      _fileModule.BuildPath(
        OLabFileStorageModule.ImportRoot,
        importFileName ),
      _fileModule.BuildPath(
        OLabFileStorageModule.ImportRoot,
        ExtractFolderName ),
      token );

    string mapJson;

    // extract the map.json file from the extracted imported files
    using ( var mapStream = new MemoryStream() )
    {
      await _fileModule.ReadFileAsync(
        mapStream,
        _fileModule.BuildPath(
          OLabFileStorageModule.ImportRoot,
          ExtractFolderName,
          MapFileName ),
          token );
      mapJson = Encoding.UTF8.GetString( mapStream.ToArray() );
    }

    // build the map object
    var mapFullDto = JsonConvert.DeserializeObject<MapsFullRelationsDto>( mapJson );

    // delete source import file
    await GetFileStorageModule().DeleteFileAsync(
      _fileModule.BuildPath(
        OLabFileStorageModule.ImportRoot,
        Path.GetFileName( importFileName ) ) );

    return mapFullDto;

  }

  private async Task<Maps> WriteMapToDatabaseAsync(
    IOLabAuthorization auth,
    MapsFullRelationsDto dto,
    CancellationToken token)
  {
    var mapDto = dto.Map;
    var phys = new MapsFullMapper( GetLogger(),
        GetDbContext(),
        GetWikiProvider() ).DtoToPhysical( mapDto );

    phys.Id = 0;
    phys.Name = $"IMPORT: {phys.Name}";
    phys.AuthorId = auth.OLabUser.Id;
    phys.CreatedAt = DateTime.UtcNow;

    await _dbContext.Maps.AddAsync( phys );
    await _dbContext.SaveChangesAsync( token );

    GetLogger().LogInformation( $"  imported map '{mapDto.Name}' {mapDto.Id.Value} -> {phys.Id}" );

    _scopedObjectPhys.AddMapIdCrossReference( mapDto.Id.Value, phys.Id );
    _scopedObjectPhys.AddScopeFromDto( dto.ScopedObjects );

    return phys;
  }

  private async Task ProcessMapNodesAsync(MapsFullRelationsDto mapFullDto, CancellationToken token)
  {
    // import the map nodes, save the new node ids for
    // when we import the map node links
    foreach ( var mapNodeDto in mapFullDto.MapNodes )
    {
      var nodePhys = await WriteMapNodeDtoToDatabase( _newMapPhys.Id, mapNodeDto, token );
      _scopedObjectPhys.AddMapNodeIdCrossReference( mapNodeDto.Id.Value, nodePhys.Id );

      _scopedObjectPhys.AddScopeFromDto( mapNodeDto.ScopedObjects );
    }

    // import the map node links
    foreach ( var mapNodeLinkDto in mapFullDto.MapNodeLinks )
    {
      var nodeLinkId = await WriteMapNodeLinkToDatabaseAsync( _newMapPhys.Id, mapNodeLinkDto, token );
      GetLogger().LogInformation( $"  imported map node link {mapNodeLinkDto.Id.Value} -> {nodeLinkId}" );
    }
  }

  private async Task CleanupImportFilesAsync()
  {
    // delete any existing import files left over
    // from previous run
    await _fileModule.DeleteFolderAsync(
      _fileModule.BuildPath(
        OLabFileStorageModule.ImportRoot,
        ExtractFolderName ) );
  }

  private async Task ProcessImportFilesAsync(
    CancellationToken token)
  {
    // list and move map-level files

    var importFilesFolder = _fileModule.BuildPath(
      OLabFileStorageModule.ImportRoot,
      ExtractFolderName,
      Api.Utils.Constants.ScopeLevelMap );

    var sourceFiles = _fileModule.GetFiles( importFilesFolder, token );

    var mapFilesFolder = _fileModule.BuildPath(
      Api.Utils.Constants.ScopeLevelMap,
      _newMapPhys.Id );

    foreach ( var sourceFile in sourceFiles )
      await _fileModule.MoveFileAsync(
      _fileModule.BuildPath(
        importFilesFolder,
        Path.GetFileName( sourceFile ) ),
      mapFilesFolder,
      token );
  }

  private async Task<MapNodes> WriteMapNodeDtoToDatabase(
    uint mapId,
    MapNodesFullDto dto,
    CancellationToken token)
  {
    var phys = new MapNodesFullMapper(
      GetLogger(),
      GetDbContext(),
      GetWikiProvider() ).DtoToPhysical( dto );

    phys.Id = 0;
    phys.MapId = mapId;

    await _dbContext.MapNodes.AddAsync( phys );
    await _dbContext.SaveChangesAsync( token );

    GetLogger().LogInformation( $"  imported map node '{phys.Title}' {dto.Id.Value} -> {phys.Id}" );

    return phys;
  }

  private async Task<uint> WriteMapNodeLinkToDatabaseAsync(
    uint mapId,
    MapNodeLinksDto dto,
    CancellationToken token)
  {
    var phys = new MapNodeLinksMapper(
      GetLogger(),
        GetDbContext(),
        GetWikiProvider() ).DtoToPhysical( dto );

    phys.Id = 0;
    phys.MapId = mapId;

    phys.NodeId1 = _scopedObjectPhys.GetMapNodeIdCrossReference( dto.SourceId.Value );
    phys.NodeId2 = _scopedObjectPhys.GetMapNodeIdCrossReference( dto.DestinationId.Value );

    await _dbContext.MapNodeLinks.AddAsync( phys );
    await _dbContext.SaveChangesAsync( token );

    return phys.Id;
  }

  private uint GetIdCrossReference(WikiTag1ArgumentModule wiki, uint sourceId)
  {
    if ( wiki is MediaResourceWikiTag )
      return _scopedObjectPhys.GetFileIdCrossReference( sourceId );

    if ( wiki is QuestionWikiTag )
      return _scopedObjectPhys.GetQuestionIdCrossReference( sourceId );

    if ( wiki is ConstantWikiTag )
      return _scopedObjectPhys.GetConstantIdCrossReference( sourceId );

    if ( wiki is CounterWikiTag )
      return _scopedObjectPhys.GetConstantIdCrossReference( sourceId );

    return _scopedObjectPhys.GetFileIdCrossReference( sourceId );
  }

  /// <summary>
  /// Replaces (deprecated) VPD tags with CONST
  /// </summary>
  /// <param name="dto">Source MapNode</param>
  /// <returns>true if found replacement</returns>
  public bool ReplaceVpdWikiTags(MapNodesFullDto dto)
  {
    var rc = false;

    var wiki = new VpdWikiTag( GetLogger(), _configuration );
    while ( wiki.HaveWikiTag( dto.Text ) )
    {
      try
      {
        var id = Convert.ToUInt32( wiki.GetWikiArgument1() );
        var newWiki = new VpdWikiTag( GetLogger(), _configuration );

        var newId = GetIdCrossReference( newWiki, id );

        newWiki.Set( "CONST", newId.ToString() );

        GetLogger().LogInformation( $"    replacing '{wiki.GetWiki()}' -> '{newWiki.GetWiki()}'" );
        dto.Text = dto.Text.Replace( wiki.GetWiki(), newWiki.GetWiki() );

        rc = true;
      }
      catch ( KeyNotFoundException )
      {
        GetLogger().LogError( $"ERROR: MapNode '{dto.Title}': could not resolve: '{wiki.GetWiki()}'" );

        dto.Text = dto.Text.Replace( wiki.GetWiki(), $"{wiki.GetUnquotedWiki()}: could not resolve" );

        rc = false;
      }
    }

    return rc;
  }

  /// <summary>
  /// Replaces (deprecated) Avatar tags with CONST
  /// </summary>
  /// <param name="dto">Source MapNode</param>
  /// <returns>true if found replacement</returns>
  public bool ReplaceAvWikiTags(MapNodesFullDto dto)
  {
    var rc = false;

    var wiki = new AvatarWikiTag( GetLogger(), _configuration );
    while ( wiki.HaveWikiTag( dto.Text ) )
    {
      var id = Convert.ToUInt16( wiki.GetWikiArgument1() );
      var newId = dto.GetIdTranslation( GetFileName(), id );

      var newWiki = new AvatarWikiTag( GetLogger(), _configuration );
      newWiki.Set( "MR", newId.Value.ToString() );

      GetLogger().LogInformation( $"    replacing '{wiki.GetWiki()}' -> '{newWiki.GetWiki()}'" );
      dto.Text = dto.Text.Replace( wiki.GetWiki(), newWiki.GetWiki() );

      rc = true;
    }

    return rc;
  }

  public bool RemapWikiTags<T>(MapNodesFullDto dto) where T : WikiTag1ArgumentModule
  {
    var rc = true;
    var mappedWikiTags = new Dictionary<string, string>();

    var wiki = (T)Activator.CreateInstance( typeof( T ), GetLogger(), _configuration );
    while ( wiki.HaveWikiTag( dto.Text ) )
    {
      var id = Convert.ToUInt32( wiki.GetWikiArgument1() );

      try
      {
        var newId = _scopedObjectPhys.GetMapNodeIdCrossReference( id );

        var newWiki = (T)Activator.CreateInstance( typeof( T ), GetLogger(), _configuration );
        newWiki.Set( wiki.GetWikiType().ToLower(), newId.Value.ToString() );

        dto.Text = dto.Text.Replace( wiki.GetWiki(), newWiki.GetWiki() );

        mappedWikiTags.Add( newWiki.GetWiki(), wiki.GetWiki() );
      }
      catch ( KeyNotFoundException )
      {
        GetLogger().LogError( $"ERROR: MapNode '{item.Title}': could not resolve: '{wiki.GetWiki()}'" );

        dto.Text = dto.Text.Replace( wiki.GetWiki(), $"{wiki.GetUnquotedWiki()}: could not resolve" );

        rc = false;
      }

    }

    foreach ( var key in mappedWikiTags.Keys )
    {
      GetLogger().LogInformation( $"    remapping '{mappedWikiTags[ key ]}' -> {key.ToUpper()}" );
      dto.Text = dto.Text.Replace( key, key.ToUpper() );
    }

    return rc;
  }


}