using Common.Utils;
using DocumentFormat.OpenXml.EMMA;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Common;
using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Utils;
using OLab.Data;
using OLab.Data.Interface;
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
  private ScopedObjects _scopedObjectPhys;
  private Maps _mapPhys;

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

      _dbContext.Database.BeginTransaction();

      // reset message buffer so we just save the new messages
      Logger.Clear();

      var mapFullDto = await ExtractImportMapDefinition(
        stream,
        fileName,
        token);

      _scopedObjectPhys = new ScopedObjects(Logger, _dbContext);

      _mapPhys = await WriteMapToDatabaseAsync(mapFullDto, token);

      await ProcessMapNodesAsync(mapFullDto, token);
      await ProcessAttachedImportFiles(token);

      await _scopedObjectPhys.WriteAllToDatabaseAsync(_mapPhys.Id, token);

      await CleanupImportAsync();

      await _dbContext.Database.CommitTransactionAsync();

    }
    catch (Exception ex)
    {
      Logger.LogError($"Import error {ex.Message}");
      await _dbContext.Database.RollbackTransactionAsync();
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
      OLabFileStorageModule.ImportRoot,
      importFileName);
    Logger.LogInformation($"Module archive file: {moduleArchiveFile}");

    // build file module extraction folder name
    ExtractFolderName = _fileModule.BuildPath(
      OLabFileStorageModule.ImportRoot,
      Path.GetFileNameWithoutExtension(importFileName));
    Logger.LogInformation($"Folder extract directory: {ExtractFolderName}");

    // delete any existing import files left over
    // from previous run
    await _fileModule.DeleteFolderAsync(ExtractFolderName);

    // save import file to storage
    await _fileModule.WriteFileAsync(
      importFileStream,
      OLabFileStorageModule.ImportRoot,
      importFileName,
      token);

    // extract import file to storage
    await _fileModule.ExtractFileToStorageAsync(
      OLabFileStorageModule.ImportRoot,
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
      OLabFileStorageModule.ImportRoot,
      Path.GetFileName(importFileName));

    return mapFullDto;

  }

  private async Task<Maps> WriteMapToDatabaseAsync(
    MapsFullRelationsDto dto,
    CancellationToken token)
  {
    var mapDto = dto.Map;
    var phys = new MapsFullMapper(Logger).DtoToPhysical(mapDto);

    phys.Id = 0;
    phys.Name = $"IMPORT: {phys.Name}";

    await _dbContext.Maps.AddAsync(phys);
    await _dbContext.SaveChangesAsync(token);

    Logger.LogInformation($"  imported map '{mapDto.Name}' {mapDto.Id.Value} -> {phys.Id}");

    _scopedObjectPhys.AddMapIdCrossReference(mapDto.Id.Value, phys.Id);
    _scopedObjectPhys.AddScopeFromDto(dto.ScopedObjects);

    return phys;
  }

  private async Task ProcessMapNodesAsync(MapsFullRelationsDto mapFullDto, CancellationToken token)
  {
    // import the map nodes, save the new node ids for
    // when we import the map node links
    foreach (var mapNodeDto in mapFullDto.MapNodes)
    {
      var nodePhys = await WriteMapNodesDtoToDatabase(_mapPhys.Id, mapNodeDto, token);
      _scopedObjectPhys.AddMapNodeIdCrossReference(mapNodeDto.Id.Value, nodePhys.Id);

      _scopedObjectPhys.AddScopeFromDto(mapNodeDto.ScopedObjects);
    }

    // import the map node links
    foreach (var mapNodeLinkDto in mapFullDto.MapNodeLinks)
    {
      var nodeLinkId = await WriteMapNodeLinkToDatabaseAsync(_mapPhys.Id, mapNodeLinkDto, token);
      Logger.LogInformation($"  imported map node link {mapNodeLinkDto.Id.Value} -> {nodeLinkId}");
    }
  }

  private async Task CleanupImportAsync()
  {
    // delete any existing import files left over
    // from previous run
    await _fileModule.DeleteFolderAsync(ExtractFolderName);
  }

  private async Task ProcessAttachedImportFiles(
    CancellationToken token)
  {
    // list and move map-level files

    var sourceFolder = _fileModule.BuildPath(
      ExtractFolderName,
      Api.Utils.Constants.ScopeLevelMap);

    var sourceFiles = _fileModule.GetFiles(sourceFolder, token);

    var destinationFolder = _fileModule.BuildPath(
      Api.Utils.Constants.ScopeLevelMap,
      _mapPhys.Id);

    foreach (var sourceFile in sourceFiles)
      await _fileModule.MoveFileAsync(
        Path.GetFileName(sourceFile),
        sourceFolder,
        destinationFolder,
        token);

    // TODO: processnode-level files

    // list and move node-level files

    //sourceFolder = _fileModule.BuildPath(
    //  ExtractFolderName,
    //  Api.Utils.ConstantsPhys.ScopeLevelNode,
    //  originalMapId);

    //sourceFiles = _fileModule.GetFiles(sourceFolder, token);

    //destinationFolder = _fileModule.BuildPath(
    //  OLabFileStorageModule.FilesRoot,
    //  Api.Utils.ConstantsPhys.ScopeLevelNode,
    //  newMapId);

    //foreach (var sourceFile in sourceFiles)
    //  await _fileModule.MoveFileAsync(
    //    Path.GetFileName(sourceFile),
    //    Path.GetDirectoryName(sourceFolder),
    //    destinationFolder,
    //    token);
  }

  private async Task<MapNodes> WriteMapNodesDtoToDatabase(
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

    Logger.LogInformation($"  imported map node '{phys.Title}' {dto.Id.Value} -> {phys.Id}");

    return phys;
  }

  private async Task<uint> WriteMapNodeLinkToDatabaseAsync(
    uint mapId,
    MapNodeLinksDto dto,
    CancellationToken token)
  {
    var phys = new MapNodeLinksMapper(
      Logger,
      _wikiTagProvider as WikiTagProvider).DtoToPhysical(dto);

    phys.Id = 0;
    phys.MapId = mapId;

    phys.NodeId1 = _scopedObjectPhys.GetMapNodeIdCrossReference(dto.SourceId.Value);
    phys.NodeId2 = _scopedObjectPhys.GetMapNodeIdCrossReference(dto.DestinationId.Value);

    await _dbContext.MapNodeLinks.AddAsync(phys);
    await _dbContext.SaveChangesAsync(token);

    return phys.Id;
  }

  //private async Task<uint> ProcessMapAsync(
  //  MapsFullRelationsDto mapFullDto,
  //  CancellationToken token)
  //{
  //  try
  //  {
  //    // import the map, get new map id
  //    var mapPhys = await WriteMapToDatabaseAsync(mapFullDto, token);

  //    // import the map nodes, save the new node ids for
  //    // when we import the map node links
  //    foreach (var mapNodeDto in mapFullDto.MapNodes)
  //    {
  //      var nodeId = await WriteMapNodesDtoToDatabase(_mapId, mapNodeDto, token);
  //      Logger.LogInformation($"  imported map node '{mapNodeDto.Title}' {mapNodeDto.Id.Value} -> {nodeId}");
  //    }

  //    // import the map nodes, save the new node ids for
  //    // when we import the map node links
  //    foreach (var mapNodeLinkDto in mapFullDto.MapNodeLinks)
  //    {
  //      var nodeLinkId = await WriteMapNodeLinkToDatabaseAsync(_mapId, mapNodeLinkDto, token);
  //      Logger.LogInformation($"  imported map node link {mapNodeLinkDto.Id.Value} -> {nodeLinkId}");
  //    }

  //    return _mapId;
  //  }

  //  catch (Exception)
  //  {
  //    await _dbContext.Database.RollbackTransactionAsync();
  //    throw;
  //  }
  //}
}