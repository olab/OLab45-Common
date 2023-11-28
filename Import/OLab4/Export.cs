using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.ObjectMapper;
using OLab.Common.Utils;
using OLab.Import.Interface;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Import.OLab4;

public partial class Importer : IImporter
{
  /// <summary>
  /// Run the export process for a map
  /// </summary>
  /// <param name="stream">Target stream for export data</param>
  /// <param name="mapId">Map id to export</param>
  /// <param name="token"></param>
  public async Task Export(
    Stream stream,
    uint mapId,
    CancellationToken token = default)
  {
    Logger.Clear();

    Logger.LogInformation($"Exporting mapId: {mapId} ");

    // create map json object
    var dto = await ExportMapAsync(mapId, token);

    // add node-level scoped objects
    await ExportMapNodesScopedObjectsAsync(dto, token);

    // serialize the dto into a json string
    var rawJson = JsonConvert.SerializeObject(dto);

    // write the json and map media files to 
    // a zip archive file
    using (var zipArchive = new ZipArchive(
      stream, 
      ZipArchiveMode.Create, 
      true))
    {
      var zipEntry = zipArchive.CreateEntry(MapFileName);

      // write the map json to the archive
      using (var mapJsonStream = new MemoryStream())
      {
        var writer = new StreamWriter(mapJsonStream);
        writer.Write(rawJson);
        writer.Flush();
        mapJsonStream.Position = 0;

        Logger.LogInformation($"Writing map '{dto.Map.Name}'. json size = {mapJsonStream.Length} ");

        using (var entryStream = zipEntry.Open())
        {
          mapJsonStream.CopyTo(entryStream);
          entryStream.Close();
        }
      }

      // add any map-level media files to the archive
      await _fileModule.CopyFolderToArchiveAsync(
        zipArchive,
        _fileModule.BuildPath(
          Api.Utils.Constants.ScopeLevelMap,
          dto.Map.Id),
        Api.Utils.Constants.ScopeLevelMap,
        true,
        token);

      // write any node-level media files to the archive
      foreach (var nodeDto in dto.MapNodes)
      {
        await _fileModule.CopyFolderToArchiveAsync(
          zipArchive,
          _fileModule.BuildPath(
            Api.Utils.Constants.ScopeLevelNode,
            nodeDto.Id),
          _fileModule.BuildPath(
            Api.Utils.Constants.ScopeLevelNode,
            nodeDto.Id),
          true,
          token);
      }
    }
  }

  private async Task<MapsFullRelationsDto> ExportMapAsync(
    uint mapId,
    CancellationToken token)
  {
    Logger.LogInformation($"  processing map {mapId} ");

    var map = await _dbContext.Maps
      .Include(map => map.MapNodes)
      .Include(map => map.MapNodeLinks)
      .AsNoTracking()
      .FirstOrDefaultAsync(
        x => x.Id == mapId,
        token);

    var dto = new MapsFullRelationsMapper(
      Logger,
      _wikiTagProvider as WikiTagProvider
    ).PhysicalToDto(map);

    // apply map-level scoped objects to the map dto
    var mapScopedObject = new Data.BusinessObjects.ScopedObjects(
      Logger,
      _dbContext,
      mapId,
      Api.Utils.Constants.ScopeLevelMap);

    var mapScopedObjectPhys = await mapScopedObject.ReadAsync(
      Api.Utils.Constants.ScopeLevelMap,
      _fileModule);

    var scopedObjectMapper = new ScopedObjectsMapper(Logger, _wikiTagProvider);
    dto.ScopedObjects = scopedObjectMapper.PhysicalToDto(mapScopedObjectPhys);

    return dto;
  }

  private async Task ExportMapNodesScopedObjectsAsync(MapsFullRelationsDto dto, CancellationToken token)
  {
    // apply node-level scoped objects to the node dtos
    foreach (var nodeDto in dto.MapNodes)
    {
      var nodeScopedObject = new Data.BusinessObjects.ScopedObjects(
        Logger,
        _dbContext,
        nodeDto.Id.Value,
        Api.Utils.Constants.ScopeLevelNode);

      Logger.LogInformation($"  processing node {nodeDto.Id} ");

      var nodeScopedObjectsPhys = await nodeScopedObject.ReadAsync(
        Api.Utils.Constants.ScopeLevelNode,
        _fileModule);

      var scopedObjectMapper = new ScopedObjectsMapper(Logger, _wikiTagProvider);
      nodeDto.ScopedObjects = scopedObjectMapper.PhysicalToDto(nodeScopedObjectsPhys);
    }
    
  }

}