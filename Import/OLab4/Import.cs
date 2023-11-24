using Common.Utils;
using Humanizer;
using Newtonsoft.Json;
using NuGet.Common;
using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.ObjectMapper;
using OLab.Import.Interface;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Import.OLab4;

public partial class Importer : IImporter
{
  // Folder where the xtracted import file resides
  private string ExtractFolderName;

  /// <summary>
  /// Run the import process on the data in the stream
  /// </summary>
  /// <param name="stream">Stream with improt archive file</param>
  /// <param name="fileName">File name of improt file</param>
  /// <param name="token"></param>
  /// <returns></returns>
  public async Task Import(
    Stream stream,
    string fileName,
    CancellationToken token = default)
  {
    try
    {
      var mapFullDto = await ExtractImportMapDefinition(
        stream,
        fileName,
        token);

      var newMapId = await ProcessMapAsync(mapFullDto, token);
      await ProcessMapFiles(mapFullDto.Map.Id.Value, newMapId, token);

      await CleanupImportAsync();
    }
    catch (Exception ex)
    {
      Logger.LogError($"Import error {ex.Message}");
      throw;
    }
  }

  /// <summary>
  /// Loads import import file into memory
  /// </summary>
  /// <param name="importFileName">Export ZIP file name</param>
  /// <returns>MapsFullRelationsDto</returns>
  private async Task<MapsFullRelationsDto> ExtractImportMapDefinition(
    Stream importFileStream,
    string importFileName,
    CancellationToken token = default)
  {
    // build file module import file name
    var moduleArchiveFile = _fileModule.BuildPath(
      _configuration.GetAppSettings().FileImportFolder,
      importFileName);
    Logger.LogInformation($"Module archive file: {moduleArchiveFile}");

    // build file module extraction folder name
    ExtractFolderName = _fileModule.BuildPath(
      _configuration.GetAppSettings().FileImportFolder,
      Path.GetFileNameWithoutExtension(importFileName));
    Logger.LogInformation($"Folder extract directory: {ExtractFolderName}");

    // delete any existing import files left over
    // from previous run
    await _fileModule.DeleteFolderAsync(ExtractFolderName);

    // save import file to storage
    await _fileModule.WriteFileAsync(
      importFileStream,
      _configuration.GetAppSettings().FileImportFolder,
      importFileName,
      token);

    // extract import file to storage
    await _fileModule.ExtractFileToStorageAsync(
      _configuration.GetAppSettings().FileImportFolder,
      importFileName,
      ExtractFolderName,
      token);

    string mapJson;

    // extract the map.json file from the extracted imported files
    using (var mapStream = new MemoryStream())
    {
      await _fileModule.ReadFileAsync(
        mapStream,
        ExtractFolderName,
        MapFileName,
        token);
      mapJson = Encoding.ASCII.GetString(mapStream.ToArray());
    }

    // build the map object
    var mapFullDto = JsonConvert.DeserializeObject<MapsFullRelationsDto>(mapJson);

    // delete source import file
    await GetFileStorageModule().DeleteFileAsync(
      _configuration.GetAppSettings().FileImportFolder,
      Path.GetFileName(importFileName));

    return mapFullDto;

  }

  private async Task<uint> ProcessMapAsync(
    MapsFullRelationsDto mapFullDto,
    CancellationToken token)
  {
    try
    {
      _dbContext.Database.BeginTransaction();

      // reset message buffer so we just save the new messages
      Logger.Clear();

      // import the map, get new map id
      _mapId = await ProcessMapRecordAsync(mapFullDto, token);

      // import the map nodes, save the new node ids for
      // when we import the map node links
      foreach (var mapNodeDto in mapFullDto.MapNodes)
      {
        var nodeId = await ProcessMapNodeRecordAsync(_mapId, mapNodeDto, token);
        Logger.LogInformation($"  imported map node '{mapNodeDto.Title}' {mapNodeDto.Id.Value} -> {nodeId}");

        _nodeIdTranslation.Add(mapNodeDto.Id.Value, nodeId);
      }

      // import the map nodes, save the new node ids for
      // when we import the map node links
      foreach (var mapNodeLinkDto in mapFullDto.MapNodeLinks)
      {
        var nodeLinkId = await ProcessMapNodeLinkRecordAsync(_mapId, mapNodeLinkDto, token);
        Logger.LogInformation($"  imported map node link {mapNodeLinkDto.Id.Value} -> {nodeLinkId}");
      }

      // import the map-level scoped objects
      await ProcessScopedObjects(
        Api.Utils.Constants.ScopeLevelMap,
        _mapId,
        mapFullDto.ScopedObjects,
        token);

      await _dbContext.Database.CommitTransactionAsync();

      return _mapId;
    }

    catch (Exception)
    {
      await _dbContext.Database.RollbackTransactionAsync();
      throw;
    }
  }

  private async Task ProcessScopedObjects(
    string scopeLevel,
    uint owningId,
    ScopedObjectsDto dto,
    CancellationToken token)
  {
    var scopedObject = new Data.BusinessObjects.ScopedObjects(
      Logger,
      _dbContext,
      owningId,
      scopeLevel);

    await scopedObject.WriteAsync(dto, token);
  }

  private async Task<uint> ProcessMapRecordAsync(
    MapsFullRelationsDto dto,
    CancellationToken token)
  {
    var mapDto = dto.Map;
    var phys = new MapsFullMapper(Logger).DtoToPhysical(mapDto);
    phys.Id = 0;

    await _dbContext.Maps.AddAsync(phys);
    await _dbContext.SaveChangesAsync(token);

    Logger.LogInformation($"  imported map '{mapDto.Name}' {mapDto.Id.Value} -> {phys.Id}");
    return phys.Id;
  }

  private async Task CleanupImportAsync()
  {
    // delete any existing import files left over
    // from previous run
    await _fileModule.DeleteFolderAsync(ExtractFolderName);
  }

  private async Task ProcessMapFiles(
    uint originalMapId,
    uint newMapId,
    CancellationToken token)
  {
    // move any map-level files
    var sourceFolder = _fileModule.BuildPath(
      ExtractFolderName,
      Api.Utils.Constants.ScopeLevelMap,
      originalMapId);

    var sourceFiles = _fileModule.GetFiles(sourceFolder, token);

    var destinationFolder = _fileModule.BuildPath(
      _configuration.GetAppSettings().FileStorageFolder,
      Api.Utils.Constants.ScopeLevelMap,
      newMapId);

    foreach (var sourceFile in sourceFiles)
      await _fileModule.MoveFileAsync(
        sourceFile,
        sourceFolder,
        destinationFolder,
        token);

    // move any node-level files
    sourceFolder = _fileModule.BuildPath(
      ExtractFolderName,
      Api.Utils.Constants.ScopeLevelNode,
      originalMapId);

    sourceFiles = _fileModule.GetFiles(sourceFolder, token);

    destinationFolder = _fileModule.BuildPath(
      _configuration.GetAppSettings().FileStorageFolder,
      Api.Utils.Constants.ScopeLevelNode,
      newMapId);

    foreach (var sourceFile in sourceFiles)
      await _fileModule.MoveFileAsync(
        sourceFile,
        sourceFolder,
        destinationFolder,
        token);
  }

  private async Task<uint> ProcessMapNodeLinkRecordAsync(
    uint mapId,
    MapNodeLinksDto dto,
    CancellationToken token)
  {
    var phys = new MapNodeLinksMapper(
      Logger,
      _wikiTagProvider as WikiTagProvider).DtoToPhysical(dto);

    phys.Id = 0;
    phys.MapId = mapId;
    phys.NodeId1 = _nodeIdTranslation[dto.SourceId.Value].Value;
    phys.NodeId2 = _nodeIdTranslation[dto.DestinationId.Value].Value;

    await _dbContext.MapNodeLinks.AddAsync(phys);
    await _dbContext.SaveChangesAsync(token);

    return phys.Id;
  }

  private async Task<uint> ProcessMapNodeRecordAsync(
    uint mapId,
    MapNodesFullDto dto,
    CancellationToken token)
  {
    var phys = new MapNodesFullMapper(
      Logger,
      _wikiTagProvider as WikiTagProvider).DtoToPhysical(dto);

    phys.Id = 0;
    phys.MapId = mapId;

    await _dbContext.MapNodes.AddAsync(phys);
    await _dbContext.SaveChangesAsync(token);

    // import the map-level scoped objects
    await ProcessScopedObjects(
      Api.Utils.Constants.ScopeLevelNode,
      phys.Id,
      dto.ScopedObjects,
      token);

    return phys.Id;
  }

}