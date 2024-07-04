using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Api.Utils;
using OLab.Api.WikiTag;
using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Data.ReaderWriters;

public partial class MapNodesReaderWriter : ReaderWriter
{
  private WikiTagModuleProvider _tagProvider;

  public static MapNodesReaderWriter Instance(IOLabLogger logger, OLabDBContext context, WikiTagModuleProvider tagProvider)
  {
    return new MapNodesReaderWriter(logger, context, tagProvider);
  }

  public MapNodesReaderWriter(IOLabLogger logger, OLabDBContext context, WikiTagModuleProvider tagProvider) : base(logger, context)
  {
    _tagProvider = tagProvider;
  }

  /// <summary>
  /// Get list of nodes
  /// </summary>
  /// <param name="ids">List of node ids</param>
  /// <returns>List of nodes</returns>
  public async Task<IList<MapNodes>> GetNodesAsync(IList<uint> ids)
  {
    var nodesPhys = await GetDbContext().MapNodes.Where(x => ids.Contains(x.Id)).ToListAsync();
    return nodesPhys;
  }

  /// <summary>
  /// Get nodes for map
  /// </summary>
  /// <param name="mapId">Map Id</param>
  /// <returns>List of map nodes</returns>
  public async Task<IList<MapNodes>> GetByMapAsync( uint mapId )
  {
    var mapNodesPhys = await GetDbContext().MapNodes.Where(x => x.MapId == mapId).ToListAsync();
    GetLogger().LogInformation(string.Format("found {0} mapNodes for {1}", mapNodesPhys.Count, mapId));
    return mapNodesPhys;
  }

  /// <summary>
  /// Get list of node ids and totles
  /// </summary>
  /// <returns>List of node ids and titles</returns>
  public IList<IdName> GetNodeIdNames()
  {
    return GetDbContext().MapNodes
      .Select(x => new IdName() { Id = x.Id, Name = x.Title }).ToList();
  }

  /// <summary>
  /// Simple get node
  /// </summary>
  /// <param name="id">Node id</param>
  /// <returns>MapNodes</returns>
  public async Task<MapNodes> GetNodeAsync(uint id)
  {
    var node = await GetDbContext().MapNodes.FirstOrDefaultAsync(x => x.Id == id);
    return node;
  }

  /// <summary>
  /// Simple get node with map id
  /// </summary>
  /// <param name="mapId">Node id</param>
  /// <param name="nodeId">Node id</param>
  /// <returns>MapNodes</returns>
  public async Task<MapNodes> GetNodeAsync(uint mapId, uint nodeId)
  {
    var node = await GetDbContext().MapNodes
     .FirstOrDefaultAsync(x => x.MapId == mapId && x.Id == nodeId);
    return node;
  }

  /// <summary>
  /// Get map root node
  /// </summary>
  /// <param name="mapId">Map id</param>
  /// <param name="nodeId">Node id</param>
  /// <returns>MapNodes</returns>
  public async Task<MapNodes> GetMapRootNode(uint mapId, uint nodeId)
  {
    if (nodeId != 0)
      return await GetDbContext().MapNodes
        .Where(x => x.MapId == mapId && x.Id == nodeId)
        .FirstOrDefaultAsync(x => x.Id == nodeId);

    var item = await GetDbContext().MapNodes
        .Where(x => x.MapId == mapId && x.TypeId == 1)
        .FirstOrDefaultAsync(x => x.Id == nodeId);

    if (item == null)
      item = await GetDbContext().MapNodes
                .Where(x => x.MapId == mapId)
                .OrderBy(x => x.Id)
                .FirstAsync();

    return item;
  }

  /// <summary>
  /// Get node for map with full relations
  /// </summary>
  /// <param name="map">Map object</param>
  /// <param name="nodeId">Node id</param>
  /// <param name="hideHidden">flag to hide hidden links</param>
  /// <param name="enableWikiTranslation">Perform Name translation</param>
  /// <returns>MapsNodesFullRelationsDto</returns>
  public async Task<MapsNodesFullRelationsDto> GetNodeAsync(
    uint mapId,
    uint nodeId,
    bool hideHidden,
    bool enableWikiTranslation)
  {
    var phys = await GetDbContext().MapNodes
      .FirstOrDefaultAsync(x => x.MapId == mapId && x.Id == nodeId);

    if (phys == null)
    {
      GetLogger().LogError($"GetNodeSync unable to find map {mapId}, node {nodeId}");
      return new MapsNodesFullRelationsDto();
    }

    // explicitly load the related objects.
    GetDbContext().Entry(phys).Collection(b => b.MapNodeLinksNodeId1Navigation).Load();

    var builder = new MapsNodesFullRelationsMapper(
      GetLogger(),
      GetDbContext(),
      _tagProvider,
      enableWikiTranslation);
    var dto = builder.PhysicalToDto(phys);

    var linkedIds =
      phys.MapNodeLinksNodeId1Navigation.Select(x => x.NodeId2).Distinct().ToList();

    var linkedNodes =
      GetDbContext().MapNodes.Where(x => linkedIds.Contains(x.Id)).ToList();

    // add destination node title to link information
    foreach (var item in dto.MapNodeLinks)
    {
      var link = linkedNodes.Where(x => x.Id == item.DestinationId).FirstOrDefault();
      item.DestinationTitle = linkedNodes
        .Where(x => x.Id == item.DestinationId)
        .Select(x => x.Title).FirstOrDefault();

      if (string.IsNullOrEmpty(item.LinkText))
        item.LinkText = item.DestinationTitle;
    }

    // if asked for, remove any hidden links
    if (hideHidden)
    {
      GetLogger().LogInformation($"GetNodeSync hiding hidden links");
      dto.MapNodeLinks = dto.MapNodeLinks.Where(x => !x.IsHidden).ToList();
    }

    return dto;
  }


}
