using Newtonsoft.Json;
using OLab.Api.Common;
using OLab.Api.Data.Interface;
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
  private ScopedObjects _scopedObjectPhys;
  private Maps _newMapPhys;

  /// <summary>
  /// Run the import process on the data in the stream
  /// </summary>
  /// <param name="archiveFileStream">Stream with improt archive file</param>
  /// <param name="archiveFileName">File name of improt file</param>
  /// <param name="token"></param>
  /// <returns></returns>
  public override async Task<Maps> Import(
    IOLabAuthorization auth,
    Stream archiveFileStream,
    string archiveFileName,
    CancellationToken token = default)
  {
    try
    {
      Authorization = auth;

      var transaction = GetDbContext().Database.BeginTransaction();

      // reset message buffer so we just save the new messages
      Logger.Clear();

      var mapFullDto = await LoadImportFromArchiveFile(
        archiveFileStream,
        archiveFileName,
        token);

      _scopedObjectPhys = new ScopedObjects(
        Logger, 
        GetDbContext(), 
        GetFileModule());

      _newMapPhys = await WriteMapToDatabaseAsync(auth, mapFullDto, token);

      await ProcessMapNodesAsync(mapFullDto, token);
      await ProcessAttachedImportFiles(archiveFileName, token);

      await _scopedObjectPhys.WriteAllToDatabaseAsync(_newMapPhys.Id, token);

      await CleanupImportAsync(archiveFileName);

      if (Logger.HasErrorMessage())
        await GetDbContext().Database.RollbackTransactionAsync();
      else
        await GetDbContext().Database.CommitTransactionAsync();

      // delete any existing import files left over
      // from previous run
      await GetFileModule().DeleteFolderAsync(
        GetFileModule().GetPhysicalImportFilePath(
          "",
          archiveFileName));

      return _newMapPhys;

    }
    catch (Exception ex)
    {
      Logger.LogError($"Import error {ex.Message}");
      await GetDbContext().Database.RollbackTransactionAsync();
      throw;
    }
  }

  /// <summary>
  /// Loads import file into memory
  /// </summary>
  /// <param name="archiveFileName">ExportAsync ZIP file name</param>
  /// <returns>MapsFullRelationsDto</returns>
  private async Task<MapsFullRelationsDto> LoadImportFromArchiveFile(
    Stream archiveFileStream,
    string archiveFileName,
    CancellationToken token = default)
  {
    Logger.LogInformation($"Module archive file: {GetFileModule().BuildPath(OLabFileStorageModule.ImportRoot, archiveFileName)}");

    var archiveFilePath = await GetFileModule().WriteImportFileAsync(
      archiveFileStream,
      archiveFileName,
      token);

    string mapJson;

    using var memoryStream = await GetFileModule().ReadImportFileAsync(
        Path.GetFileNameWithoutExtension(archiveFileName),
        MapFileName);
    mapJson = Encoding.UTF8.GetString(memoryStream.ToArray());

    // build the map object
    var mapFullDto = JsonConvert.DeserializeObject<MapsFullRelationsDto>(mapJson);

    // delete source archive file 
    await GetFileModule().DeleteImportFileAsync(
      ".",
      archiveFileName);

    return mapFullDto;

  }

  private async Task<Maps> WriteMapToDatabaseAsync(
    IOLabAuthorization auth,
    MapsFullRelationsDto dto,
    CancellationToken token)
  {
    var mapDto = dto.Map;
    var phys = new MapsFullMapper(Logger).DtoToPhysical(mapDto);

    phys.Id = 0;
    phys.Name = $"IMPORT: {phys.Name}";
    phys.AuthorId = auth.UserContext.UserId;

    await GetDbContext().Maps.AddAsync(phys);
    await GetDbContext().SaveChangesAsync(token);

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
      var nodePhys = await WriteMapNodesDtoToDatabase(_newMapPhys.Id, mapNodeDto, token);
      _scopedObjectPhys.AddMapNodeIdCrossReference(mapNodeDto.Id.Value, nodePhys.Id);

      _scopedObjectPhys.AddScopeFromDto(mapNodeDto.ScopedObjects);
    }

    // import the map node links
    foreach (var mapNodeLinkDto in mapFullDto.MapNodeLinks)
    {
      var nodeLinkId = await WriteMapNodeLinkToDatabaseAsync(_newMapPhys.Id, mapNodeLinkDto, token);
      Logger.LogInformation($"  imported map node link {mapNodeLinkDto.Id.Value} -> {nodeLinkId}");
    }
  }

  private async Task CleanupImportAsync(string archiveFileName)
  {
    // delete any existing import files left over
    // from previous run
    await GetFileModule().DeleteFolderAsync(
      GetFileModule().GetPhysicalImportFilePath(
        "",
        archiveFileName));
  }

  private async Task ProcessAttachedImportFiles(
    string archiveFileName,
    CancellationToken token)
  {
    // list and move map-level files

    var importFilesFolder = GetFileModule().BuildPath(
      OLabFileStorageModule.ImportRoot,
      archiveFileName,
      Api.Utils.Constants.ScopeLevelMap);

    var sourceFiles = GetFileModule().GetFiles(importFilesFolder, token);

    var mapFilesFolder = GetFileModule().BuildPath(
      OLabFileStorageModule.FilesRoot,
      Api.Utils.Constants.ScopeLevelMap,
      _newMapPhys.Id);

    foreach (var sourceFile in sourceFiles)
      await GetFileModule().MoveFileAsync(
      GetFileModule().BuildPath(
        importFilesFolder,
        Path.GetFileName(sourceFile)),
      mapFilesFolder,
      token);
  }

  private async Task<MapNodes> WriteMapNodesDtoToDatabase(
    uint mapId,
    MapNodesFullDto dto,
    CancellationToken token)
  {
    var phys = new MapNodesFullMapper(
      Logger,
      GetWikiProvider() as WikiTagProvider).DtoToPhysical(dto);

    phys.Id = 0;
    phys.MapId = mapId;

    await GetDbContext().MapNodes.AddAsync(phys);
    await GetDbContext().SaveChangesAsync(token);

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
      GetWikiProvider() as WikiTagProvider).DtoToPhysical(dto);

    phys.Id = 0;
    phys.MapId = mapId;

    phys.NodeId1 = _scopedObjectPhys.GetMapNodeIdCrossReference(dto.SourceId.Value);
    phys.NodeId2 = _scopedObjectPhys.GetMapNodeIdCrossReference(dto.DestinationId.Value);

    await GetDbContext().MapNodeLinks.AddAsync(phys);
    await GetDbContext().SaveChangesAsync(token);

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
  //    await GetDbContext().Database.RollbackTransactionAsync();
  //    throw;
  //  }
  //}
}