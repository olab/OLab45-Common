using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.ObjectMapper;
using OLab.Data;
using OLab.Import.Interface;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Import.OLab4;

public partial class Importer : IImporter
{
  /// <summary>
  /// Export a map to json
  /// </summary>
  /// <param name="mapId">Map id to export</param>
  /// <param name="token"></param>
  /// <returns>MapsFullRelationsDto</returns>
  public override async Task<MapsFullRelationsDto> ExportAsync(
    uint mapId,
    CancellationToken token = default)
  {
    Logger.LogInformation($"Exporting mapId: {mapId} ");

    // importer must be a superuser or an importer
    if (!auth.IsMemberOf("*", Api.Model.Roles.RoleNameSuperuser)
      && !auth.IsMemberOf("*", Api.Model.Roles.RoleNameImporter))
      throw new OLabUnauthorizedException();

    // create map json object
    var dto = await ReadMapDtoFromDatabase(mapId, token);

    // add node-level scoped objects
    await ReadMapNodeScopedObjectFromDatabase(dto, token);

    return dto;
  }

  /// <summary>
  /// Run the export process for a map
  /// </summary>
  /// <param name="stream">Target stream for export data</param>
  /// <param name="mapId">Map id to export</param>
  /// <param name="token"></param>
  public override async Task ExportAsync(
    Stream stream,
    uint mapId,
    CancellationToken token = default)
  {
    Logger.Clear();

    // importer must be a superuser or an importer
    if (!auth.IsMemberOf("*", Api.Model.Roles.RoleNameSuperuser)
      && !auth.IsMemberOf("*", Api.Model.Roles.RoleNameImporter))
      throw new OLabUnauthorizedException();

    // create map json object
    var dto = await ExportAsync(auth, mapId, token);

    // serialize the dto into a json string
    var rawJson = JsonConvert.SerializeObject(dto);

    // write the json and map media files to 
    // a zip archive file
    using var zipArchive = new ZipArchive(
      stream,
      ZipArchiveMode.Create,
      true);
    var zipEntry = zipArchive.CreateEntry(MapFileName);

    // write the map json to the archive
    using (var mapJsonStream = new MemoryStream())
    {
      var writer = new StreamWriter(mapJsonStream);
      writer.Write(rawJson);
      writer.Flush();
      mapJsonStream.Position = 0;

      Logger.LogInformation($"Writing map '{dto.Map.Name}'. json size = {mapJsonStream.Length} ");

      using var entryStream = zipEntry.Open();
      mapJsonStream.CopyTo(entryStream);
      entryStream.Close();
    }

    // add any map-level media files to the archive
    await GetFileModule().CopyFolderToArchiveAsync(
      zipArchive,
      GetFileModule().GetPhysicalScopedFilePath(
        Api.Utils.Constants.ScopeLevelMap,
        dto.Map.Id.Value),
      Api.Utils.Constants.ScopeLevelMap,
      true,
      token);

    // write any node-level media files to the archive
    foreach (var nodeDto in dto.MapNodes)
    {
      await GetFileModule().CopyFolderToArchiveAsync(
        zipArchive,
        GetFileModule().GetPhysicalScopedFilePath(
          Api.Utils.Constants.ScopeLevelNode,
          nodeDto.Id.Value),
        GetFileModule().BuildPath(
          Api.Utils.Constants.ScopeLevelNode,
          nodeDto.Id),
        true,
        token);
    }
  }

  private async Task<MapsFullRelationsDto> ReadMapDtoFromDatabase(
    uint mapId,
    CancellationToken token)
  {
    Logger.LogInformation($"  exporting map {mapId} ");

    var map = await GetDbContext().Maps
      .Include(map => map.MapNodes)
      .Include(map => map.MapNodeLinks)
      .AsNoTracking()
      .FirstOrDefaultAsync(
        x => x.Id == mapId,
        token);

    if (map == null)
      throw new OLabObjectNotFoundException("Maps", mapId);

    var dto = new MapsFullRelationsMapper(
      Logger,
      GetWikiProvider() as WikiTagProvider,
      false
    ).PhysicalToDto(map);

    var phys = new ScopedObjects(
      Logger,
      GetDbContext(),
      GetFileModule());

    // apply map-level scoped objects to the map dto
    await phys.AddScopeFromDatabaseAsync(Api.Utils.Constants.ScopeLevelMap, mapId);

    var scopedObjectMapper = new ScopedObjectsMapper(Logger, GetWikiProvider(), false);
    dto.ScopedObjects = scopedObjectMapper.PhysicalToDto(phys);

    return dto;
  }

  private async Task ReadMapNodeScopedObjectFromDatabase(MapsFullRelationsDto dto, CancellationToken token)
  {
    // apply node-level scoped objects to the node dtos
    foreach (var nodeDto in dto.MapNodes)
    {
      var phys = new ScopedObjects(
        Logger,
        GetDbContext(),
        GetFileModule());

      // apply node-level scoped objects
      await phys.AddScopeFromDatabaseAsync(Api.Utils.Constants.ScopeLevelNode, nodeDto.Id.Value);

      Logger.LogInformation($"  exporting node {nodeDto.Id} ");

      var scopedObjectMapper = new ScopedObjectsMapper(Logger, GetWikiProvider(), false);
      nodeDto.ScopedObjects = scopedObjectMapper.PhysicalToDto(phys);
    }

  }

}