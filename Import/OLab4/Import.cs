using Humanizer;
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
using System.Linq;
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

      var nodePhysList = await ProcessMapNodesAsync( mapFullDto, token );
      await ProcessImportFilesAsync( token );

      await _scopedObjectPhys.WriteAllToDatabaseAsync( _newMapPhys.Id, token );
      ProcessMapNodesScopedObjects( mapFullDto, token );

      await UpdateMapNodesInDatabaseAsync( auth, nodePhysList, mapFullDto.MapNodes, token );

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

  private void ProcessMapNodesScopedObjects(MapsFullRelationsDto mapFullDto, CancellationToken token)
  {
    // import the map nodes, save the new node ids for
    // when we import the map node links
    foreach ( var mapNodeDto in mapFullDto.MapNodes )
    {
      GetLogger().LogInformation( $"  post processing map node '{mapNodeDto.Title}' {mapNodeDto.Id.Value}" );

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

  private async Task UpdateMapNodesInDatabaseAsync(
    IOLabAuthorization auth,
    IList<MapNodes> mapNodesPhys,
    IList<MapNodesFullDto> mapNodesDto,
    CancellationToken token)
  {
    foreach ( var mapNodeDto in mapNodesDto )
    {
      var mapNodePhysId = _scopedObjectPhys.GetMapNodeIdCrossReference( mapNodeDto.Id.Value );

      var mapNodePhys = mapNodesPhys.FirstOrDefault( x => x.Id == mapNodePhysId );
      if ( mapNodePhys == null )
      {
        GetLogger().LogError( $"ERROR: MapNode could not find map node {mapNodeDto.Id}" );
        continue;
      }

      mapNodePhys.Text = mapNodeDto.Text;

      _dbContext.MapNodes.Update( mapNodePhys );
      await _dbContext.SaveChangesAsync( token );

      GetLogger().LogInformation( $"  updated '{mapNodePhys.Title}' node text" );

    }
  }

  private async Task<IList<MapNodes>> ProcessMapNodesAsync(MapsFullRelationsDto mapFullDto, CancellationToken token)
  {
    var nodePhysList = new List<MapNodes>();

    // import the map nodes, save the new node ids for
    // when we import the map node links
    foreach ( var mapNodeDto in mapFullDto.MapNodes )
    {
      var nodePhys = await WriteMapNodeDtoToDatabase( _newMapPhys.Id, mapNodeDto, token );
      _scopedObjectPhys.AddMapNodeIdCrossReference( mapNodeDto.Id.Value, nodePhys.Id );

      _scopedObjectPhys.AddScopeFromDto( mapNodeDto.ScopedObjects );

      nodePhysList.Add( nodePhys );
    }

    // import the map node links
    foreach ( var mapNodeLinkDto in mapFullDto.MapNodeLinks )
    {
      var nodeLinkId = await WriteMapNodeLinkToDatabaseAsync( _newMapPhys.Id, mapNodeLinkDto, token );
      GetLogger().LogInformation( $"  imported map node link {mapNodeLinkDto.Id.Value} -> {nodeLinkId}" );
    }

    return nodePhysList;
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

  private string GetIdCrossReference(WikiTag1ArgumentModule wiki, string id)
  {
    if ( wiki is MediaResourceWikiTag )
      return _scopedObjectPhys.GetFileCrossReference( id );

    if ( wiki is QuestionWikiTag )
      return _scopedObjectPhys.GetQuestionCrossReference( id );

    if ( wiki is ConstantWikiTag )
      return _scopedObjectPhys.GetConstantCrossReference( id );

    if ( wiki is CounterWikiTag )
      return _scopedObjectPhys.GetCounterCrossReference( id );

    return _scopedObjectPhys.GetFileCrossReference( id );
  }

  /// <summary>
  /// Replaces (deprecated) VPD tags with CONST
  /// </summary>
  /// <param name="dto">Source MapNode</param>
  /// <returns>true if found replacement</returns>
  public bool ReplaceVpdWikiTags(MapNodesFullDto dto)
  {
    var rc = false;
    var mappedWikiTags = new Dictionary<string, string>();

    var originalWiki = new VpdWikiTag( GetLogger(), _configuration );
    while ( originalWiki.HaveWikiTag( dto.Text ) )
    {
      try
      {
        var id = originalWiki.GetWikiArgument1();
        var newWiki = new VpdWikiTag( GetLogger(), _configuration );

        var newId = GetIdCrossReference( newWiki, id );

        newWiki.Set( "const", newId.ToString() );

        GetLogger().LogInformation( $"    replacing '{originalWiki.GetWiki()}' -> '{newWiki.GetWiki()}'" );
        dto.Text = dto.Text.Replace( originalWiki.GetWiki(), newWiki.GetWiki() );

        rc = true;
        mappedWikiTags.Add( newWiki.GetWiki(), originalWiki.GetWiki() );

      }
      catch ( KeyNotFoundException )
      {
        GetLogger().LogError( $"ERROR: MapNode '{dto.Title}': could not resolve: '{originalWiki.GetWiki()}'" );

        dto.Text = dto.Text.Replace( originalWiki.GetWiki(), $"{originalWiki.GetUnquotedWiki()}: could not resolve" );

        rc = false;
      }
    }

    // remap the wiki tags to upper case so they are valid
    foreach ( var key in mappedWikiTags.Keys )
    {
      GetLogger().LogInformation( $"    remapping '{mappedWikiTags[ key ]}' -> {key.ToUpper()}" );
      dto.Text = dto.Text.Replace( key, key.ToUpper() );
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
    var mappedWikiTags = new Dictionary<string, string>();

    var originalWiki = new AvatarWikiTag( GetLogger(), _configuration );
    while ( originalWiki.HaveWikiTag( dto.Text ) )
    {
      var id = originalWiki.GetWikiArgument1();
      var newWiki = new AvatarWikiTag( GetLogger(), _configuration );

      var newId = GetIdCrossReference( newWiki, id );

      newWiki.Set( "mr", newId.ToString() );

      GetLogger().LogInformation( $"    replacing '{originalWiki.GetWiki()}' -> '{newWiki.GetWiki()}'" );
      dto.Text = dto.Text.Replace( originalWiki.GetWiki(), newWiki.GetWiki() );

      rc = true;
      mappedWikiTags.Add( newWiki.GetWiki(), originalWiki.GetWiki() );

    }

    // remap the wiki tags to upper case so they are valid
    foreach ( var key in mappedWikiTags.Keys )
    {
      GetLogger().LogInformation( $"    remapping '{mappedWikiTags[ key ]}' -> {key.ToUpper()}" );
      dto.Text = dto.Text.Replace( key, key.ToUpper() );
    }

    return rc;
  }

  public bool RemapWikiTags<T>(MapNodesFullDto dto) where T : WikiTag1ArgumentModule
  {
    var rc = true;
    var mappedWikiTags = new Dictionary<string, string>();

    var originalWiki = (T)Activator.CreateInstance( typeof( T ), GetLogger(), _configuration );
    while ( originalWiki.HaveWikiTag( dto.Text ) )
    {
      try
      {
        var wikiArgument1 = originalWiki.GetWikiArgument1();
        var newWiki = (T)Activator.CreateInstance( typeof( T ), GetLogger(), _configuration );

        var newId = GetIdCrossReference( newWiki, wikiArgument1 );

        // set the wiki using ToLower to prevent the wiki from being 
        // being matched again in the while loop
        newWiki.Set( originalWiki.GetWikiType().ToLower(), newId.ToString() );

        GetLogger().LogInformation( $"    remapping '{originalWiki.GetWiki()}' -> [[{newWiki.GetWikiType().ToUpper()}:{newWiki.GetWikiId()}]]" );

        dto.Text = dto.Text.Replace( originalWiki.GetWiki(), newWiki.GetWiki() );

        originalWiki.SetWikiId( newWiki.GetWikiId() );
        mappedWikiTags.Add( newWiki.GetWiki(), originalWiki.GetWiki() );

      }
      catch ( KeyNotFoundException )
      {
        GetLogger().LogError( $"ERROR: {typeof( T ).Name}: could not resolve: '{originalWiki.GetWiki()}'" );
        dto.Text = dto.Text.Replace( originalWiki.GetWiki(), $"{originalWiki.GetUnquotedWiki()}: could not resolve" );
        rc = false;
      }

    }

    // remap the wiki tags to upper case so they are valid
    foreach ( var key in mappedWikiTags.Keys )
      dto.Text = dto.Text.Replace( key, mappedWikiTags[ key ] );

    return rc;
  }

}
