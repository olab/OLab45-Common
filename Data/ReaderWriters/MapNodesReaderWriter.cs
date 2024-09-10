using AutoMapper.Internal.Mappers;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using OLab.Api.Common;
using OLab.Api.Data.Exceptions;
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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Linq;

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
  public async Task<IList<MapNodes>> GetByMapAsync(uint mapId)
  {
    var mapNodesPhys = await GetDbContext()
      .MapNodes
      .Include("Map")
      .Where(x => x.MapId == mapId).ToListAsync();
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
    var node = await GetDbContext().MapNodes
      .Include("MapNodeGrouproles")
      .FirstOrDefaultAsync(x => x.Id == id);

    // catch case of an uninitialized node with no group roles
    await InitializeGroupRolesAsync(node);

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
     .Include(c => c.MapNodeGrouproles).ThenInclude( d => d.Group )
     .Include(c => c.MapNodeGrouproles).ThenInclude(d => d.Role)
     .FirstOrDefaultAsync(x => x.MapId == mapId && x.Id == nodeId);

    // catch case of an uninitialized node with no group roles
    await InitializeGroupRolesAsync(node);

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
    MapNodes node = null;

    if (nodeId != 0)
      node = await GetDbContext().MapNodes
        .Include("MapNodeGrouproles")
        .Where(x => x.MapId == mapId && x.Id == nodeId)
        .FirstOrDefaultAsync(x => x.Id == nodeId);
    else
      node = await GetDbContext().MapNodes
          .Include("MapNodeGrouproles")
          .Where(x => x.MapId == mapId && x.TypeId == 1)
          .FirstOrDefaultAsync(x => x.Id == nodeId);

    // if no explicit map root node, get first node 
    // in database
    if (node == null)
      node = await GetDbContext().MapNodes
        .Include("MapNodeGrouproles")
        .Where(x => x.MapId == mapId)
        .OrderBy(x => x.Id)
      .FirstAsync();

    // catch case of an uninitialized node with no group roles
    await InitializeGroupRolesAsync(node);

    return node;
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
    var phys = await GetMapRootNode(mapId, nodeId);

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
      GetDbContext().MapNodes
        .Include("MapNodeGrouproles")
        .Where(x => linkedIds.Contains(x.Id)).ToList();

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

  /// <summary>
  /// If a legacy map with no group roles, initialize
  /// node with all-access record
  /// </summary>
  /// <param name="node">Node to evaluate</param>
  private async Task InitializeGroupRolesAsync(MapNodes node)
  {
    if (node.MapNodeGrouproles.Count > 0)
      return;

    GetLogger().LogInformation($"initializing group/role for node {node.Title}({node.Id})");

    node.MapNodeGrouproles.Add(new MapNodeGrouproles { Id = 0, GroupId = null, RoleId = null });

    GetDbContext().MapNodes.Update(node);
    await GetDbContext().SaveChangesAsync();
  }

  /// <summary>
  /// Edits a node
  /// </summary>
  /// <param name="dto">Map node dto</param>
  /// <param name="wikiProvider">wikitag provider</param>
  /// <param name="save">commit on every write</param>
  /// <returns>id of edited node</returns>
  public async Task<uint> PutNodeAsync(
    MapNodesFullDto dto,
    WikiTagModuleProvider wikiProvider,
    bool save = true
  )
  {
    using var transaction = GetDbContext().Database.BeginTransaction();

    try
    {
      var builder = new MapNodesFullMapper(
        GetLogger(),
      GetDbContext(),
        wikiProvider);
      var newMapNodePhys = builder.DtoToPhysical(dto);

      // entity as it currently exists in the db
      var dbMapNodePhys = GetDbContext().MapNodes.Include(c => c.MapNodeGrouproles)
          .FirstOrDefault(g => g.Id == newMapNodePhys.Id);
      // update properties on the parent
      GetDbContext().Entry(dbMapNodePhys).CurrentValues.SetValues(newMapNodePhys);

      dbMapNodePhys.MapNodeGrouproles.Clear();
      foreach (var groupRolePhys in newMapNodePhys.MapNodeGrouproles)
        dbMapNodePhys.MapNodeGrouproles.Add(
          new MapNodeGrouproles
          {
            GroupId = groupRolePhys.GroupId,
            NodeId = groupRolePhys.NodeId,
            RoleId = groupRolePhys.RoleId
          }
        );

      if (save)
        await GetDbContext().SaveChangesAsync();

      await transaction.CommitAsync();

    }
    catch (Exception)
    {
      await transaction.RollbackAsync();
      throw;
    }

    return dto.Id.Value;

  }

  /// <summary>
  /// Delete a node
  /// </summary>
  /// <param name="nodeId">Node id</param>
  /// <returns>node id deleted</returns>
  public async Task<uint> DeleteNodeAsync(
    uint nodeId
  )
  {
    using var transaction = GetDbContext().Database.BeginTransaction();

    try
    {
      var links = GetDbContext().MapNodeLinks.
        Where(x => (x.NodeId1 == nodeId) || (x.NodeId2 == nodeId)).ToArray();

      GetLogger().LogInformation($"deleting {links.Count()} links");
      GetDbContext().MapNodeLinks.RemoveRange(links);

      var node = await GetDbContext().MapNodes.FirstOrDefaultAsync(x => x.Id == nodeId);

      if (node == null)
        throw new OLabObjectNotFoundException("MapNodes", nodeId);

      GetDbContext().MapNodes.Remove(node);
      GetLogger().LogInformation($"deleting node id: {node.Id}");

      await GetDbContext().SaveChangesAsync();

      await transaction.CommitAsync();

    }
    catch (Exception)
    {
      await transaction.RollbackAsync();
      throw;
    }

    return nodeId;

  }


}
