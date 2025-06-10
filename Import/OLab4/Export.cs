using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OLab.Api.Data.Exceptions;
using OLab.Api.Dto;
using OLab.Api.ObjectMapper;
using OLab.Common.Utils;
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
  public async Task<MapsFullRelationsDto> ExportAsync(
    uint mapId,
    CancellationToken token = default)
  {
    GetLogger().LogInformation( $"Exporting mapId: {mapId} " );

    // create map json object
    var dto = await ReadMapDtoFromDatabase( mapId, token );

    // add node-level scoped objects
    await ReadMapNodeScopedObjectFromDatabase( dto, token );

    return dto;
  }

  /// <summary>
  /// Run the export process for a map
  /// </summary>
  /// <param name="stream">Target stream for export data</param>
  /// <param name="mapId">Map id to export</param>
  /// <param name="token"></param>
  public async Task ExportAsync(
    Stream stream,
    uint mapId,
    CancellationToken token = default)
  {
    GetLogger().Clear();

    // create map json object
    var dto = await ExportAsync( mapId, token );

    // serialize the dto into a json string
    var rawJson = JsonConvert.SerializeObject( dto );

    // write the json and map media files to 
    // a zip archive file
    using var zipArchive = new ZipArchive(
      stream,
      ZipArchiveMode.Create,
      true );

    var zipEntry = zipArchive.CreateEntry( MapFileName );

    // write the map json to the archive
    using ( var mapJsonStream = new MemoryStream() )
    {
      var writer = new StreamWriter( mapJsonStream );
      writer.Write( rawJson );
      writer.Flush();
      mapJsonStream.Position = 0;

      GetLogger().LogInformation( $"Writing map '{dto.Map.Name}'. json size = {mapJsonStream.Length} " );

      using var entryStream = zipEntry.Open();
      mapJsonStream.CopyTo( entryStream );
      entryStream.Close();
    }

    // add any map-level media files to the archive
    await _fileModule.CopyFolderToArchiveAsync(
      zipArchive,
      _fileModule.BuildPath(
        Api.Utils.Constants.ScopeLevelMap,
        dto.Map.Id ),
      Api.Utils.Constants.ScopeLevelMap,
      true,
      token );

    // write any node-level media files to the archive
    foreach ( var nodeDto in dto.MapNodes )
    {
      await _fileModule.CopyFolderToArchiveAsync(
        zipArchive,
        _fileModule.BuildPath(
          Api.Utils.Constants.ScopeLevelNode,
          nodeDto.Id ),
        _fileModule.BuildPath(
          Api.Utils.Constants.ScopeLevelNode,
          nodeDto.Id ),
        true,
        token );
    }
  }

  private async Task<MapsFullRelationsDto> ReadMapDtoFromDatabase(
    uint mapId,
    CancellationToken token)
  {
    GetLogger().LogInformation( $"  exporting map {mapId} " );

    var map = await GetDbContext().Maps
      .Include( map => map.MapNodes )
      .Include( map => map.MapNodeLinks )
      .AsNoTracking()
      .FirstOrDefaultAsync(
        x => x.Id == mapId,
        token );

    if ( map == null )
      throw new OLabObjectNotFoundException( Api.Utils.Constants.ScopeLevelMap, mapId );

    var dto = new MapsFullRelationsMapper(

        GetLogger(),
        GetDbContext(),
        GetWikiProvider(),
      false
    ).PhysicalToDto( map );

    var phys = new ScopedObjects(
      GetLogger(), GetDbContext(), GetWikiProvider() );

    // apply map-level scoped objects to the map dto
    await phys.LoadScopedObjectsFromDatabaseAsync( Api.Utils.Constants.ScopeLevelMap, mapId );

    var scopedObjectMapper = new ScopedObjectsMapper(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider(), false );
    dto.ScopedObjects = scopedObjectMapper.PhysicalToDto( phys );

    return dto;
  }

  private async Task ReadMapNodeScopedObjectFromDatabase(MapsFullRelationsDto dto, CancellationToken token)
  {
    // apply node-level scoped objects to the node dtos
    foreach ( var nodeDto in dto.MapNodes )
    {
      var phys = new ScopedObjects(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider() );

      // apply node-level scoped objects
      await phys.LoadScopedObjectsFromDatabaseAsync( Api.Utils.Constants.ScopeLevelNode, nodeDto.Id.Value );

      GetLogger().LogInformation( $"  exporting node {nodeDto.Id} " );

      var scopedObjectMapper = new ScopedObjectsMapper(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider(), false );
      nodeDto.ScopedObjects = scopedObjectMapper.PhysicalToDto( phys );
    }

  }

}