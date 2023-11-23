using Common.Utils;
using Newtonsoft.Json;
using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.ObjectMapper;
using OLab.Import.Interface;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Import.OLab4;

public partial class Importer : IImporter
{
  // Folder where th extracted Import file resides
  private string ExtractFolderName;

  public async Task Import(
    Stream importFileStream,
    string importFileName,
    CancellationToken token = default)
  {
    try
    {
      var mapFullDto = await ProcessImportFileAsync(
        importFileStream, 
        importFileName, 
        token);

      var newMapId = await ProcessMapAsync(mapFullDto, token);
      await ProcessMapFiles(mapFullDto.Map.Id.Value, newMapId, token);

      CleanupImport();
    }
    catch (Exception ex)
    {
      Logger.LogError($"Import error {ex.Message}");
      throw;
    }
  }

  /// <summary>
  /// Loads import xml files into memory
  /// </summary>
  /// <param name="importFileName">Export ZIP file name</param>
  /// <returns>true</returns>
  private async Task<MapsFullRelationsDto> ProcessImportFileAsync(
    Stream importFileStream,
    string importFileName,
    CancellationToken token = default)
  {
    var moduleArchiveFile = _fileModule.BuildPath(
      _configuration.GetAppSettings().FileImportFolder,
      importFileName); ;
    Logger.LogInformation($"Module archive file: {moduleArchiveFile}");

    // save import file to storage
    await _fileModule.CopyStreamToFileAsync(importFileStream, moduleArchiveFile, token);

    ExtractFolderName = _fileModule.BuildPath(
      _configuration.GetAppSettings().FileImportFolder,
      Path.GetFileNameWithoutExtension(importFileName).Replace(".", ""));
    Logger.LogInformation($"Folder extract directory: {ExtractFolderName}");

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
      await _fileModule.CopyFileToStreamAsync(
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

      await _dbContext.Database.CommitTransactionAsync();

      return _mapId;
    }

    catch (Exception)
    {
      await _dbContext.Database.RollbackTransactionAsync();
      throw;
    }
  }

  private async Task<uint> ProcessMapRecordAsync(
    MapsFullRelationsDto mapFullDto,
    CancellationToken token)
  {
    var mapDto = mapFullDto.Map;
    var phys = new MapsFullMapper(Logger).DtoToPhysical(mapDto);
    phys.Id = 0;

    await _dbContext.Maps.AddAsync(phys);
    await _dbContext.SaveChangesAsync(token);

    Logger.LogInformation($"  imported map '{mapDto.Name}' {mapDto.Id.Value} -> {phys.Id}");
    return phys.Id;
  }

  private async Task ProcessMapFiles(uint originalMapId, uint newMapId, CancellationToken token)
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
    MapNodeLinksDto mapNodeLinkDto,
    CancellationToken token)
  {
    var phys = new MapNodeLinksMapper(
      Logger,
      _wikiTagProvider as WikiTagProvider).DtoToPhysical(mapNodeLinkDto);

    phys.Id = 0;
    phys.MapId = mapId;
    phys.NodeId1 = _nodeIdTranslation[mapNodeLinkDto.SourceId.Value].Value;
    phys.NodeId2 = _nodeIdTranslation[mapNodeLinkDto.DestinationId.Value].Value;

    await _dbContext.MapNodeLinks.AddAsync(phys);
    await _dbContext.SaveChangesAsync(token);

    return phys.Id;
  }

  private async Task<uint> ProcessMapNodeRecordAsync(
    uint mapId,
    MapNodesFullDto mapNodeFullDto,
    CancellationToken token)
  {
    var phys = new MapNodesFullMapper(
      Logger,
      _wikiTagProvider as WikiTagProvider).DtoToPhysical(mapNodeFullDto);

    phys.Id = 0;
    phys.MapId = mapId;

    await _dbContext.MapNodes.AddAsync(phys);
    await _dbContext.SaveChangesAsync(token);

    return phys.Id;
  }

  private void CleanupImport()
  {
    Directory.Delete(ExtractFolderName, true);
  }

}