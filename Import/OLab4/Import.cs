using Newtonsoft.Json;
using OLab.Access.Interfaces;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Utils;
using OLab.Data;
using OLab.Import.Interface;
using System;
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

}