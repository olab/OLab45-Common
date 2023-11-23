using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OLab.Api.Common;
using OLab.Api.ObjectMapper;
using OLab.Import.Interface;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Import.OLab4;

public partial class Importer : IImporter
{
  public async Task Export(Stream stream, uint mapId, CancellationToken token = default)
  {
    Logger.LogInformation($"Exporting mapId: {mapId} ");

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

    var mapScopedObjectPhys = await mapScopedObject.GetAsync(
      Api.Utils.Constants.ScopeLevelMap, 
      _fileModule);

    var scopedObjectMapper = new ScopedObjectsMapper(Logger, _wikiTagProvider);
    dto.ScopedObjects = scopedObjectMapper.PhysicalToDto(mapScopedObjectPhys);

    // apply node-level scoped objects to the node dtos
    foreach (var nodeDto in dto.MapNodes)
    {
      var nodeScopedObject = new Data.BusinessObjects.ScopedObjects(
        Logger, 
        _dbContext, 
        nodeDto.Id.Value, 
        Api.Utils.Constants.ScopeLevelNode);

      var nodeScopedObjectsPhys = await nodeScopedObject.GetAsync(
        Api.Utils.Constants.ScopeLevelNode, 
        _fileModule);

      nodeDto.ScopedObjects = scopedObjectMapper.PhysicalToDto(nodeScopedObjectsPhys);
    }

    var rawJson = JsonConvert.SerializeObject(dto);

    using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, true))
    {
      var entry = zipArchive.CreateEntry(MapFileName);

      // write the map json to the archive
      using (var fileContentStream = new MemoryStream())
      {
        var writer = new StreamWriter(fileContentStream);
        writer.Write(rawJson);
        writer.Flush();

        fileContentStream.Position = 0;

        Logger.LogInformation($"Read map: {map.Name} into stream. json size = {fileContentStream.Length} ");

        using (var entryStream = entry.Open())
        {
          fileContentStream.CopyTo(entryStream);
          entryStream.Close();
        }
      }

      // add any map-level media files to the archive
      await _fileModule.CopyFoldertoArchiveAsync(
        zipArchive,
        $"{Api.Utils.Constants.ScopeLevelMap}{_fileModule.GetFolderSeparator()}{map.Id}",
        true,
        token);

      // write any node-level media files to the archive
      foreach (var nodeDto in dto.MapNodes)
        await _fileModule.CopyFoldertoArchiveAsync(
          zipArchive,
          $"{Api.Utils.Constants.ScopeLevelNode}{_fileModule.GetFolderSeparator()}{nodeDto.Id}",
          true,
          token);
    }
  }
}