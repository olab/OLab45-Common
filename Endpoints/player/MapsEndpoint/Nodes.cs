using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Data.ReaderWriters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints.Player;

public partial class MapsEndpoint : OLabEndpoint
{
  /// <summary>
  /// Gets a node, with no security
  /// </summary>
  /// <param name="mapId">Map Id</param>
  /// <param name="nodeId">Node Id</param>
  /// <param name="hideHidden">Flag to hide hidden links</param>
  /// <param name="enableWikiTranslation">Flag to (dis)able wiki tag translation</param>
  /// <returns>MapsNodesFullRelationsDto</returns>
  /// <exception cref="OLabObjectNotFoundException"></exception>
  /// <exception cref="OLabGeneralException"></exception>
  public async Task<MapsNodesFullRelationsDto> GetRawNodeAsync(
    uint mapId,
    uint nodeId,
    bool hideHidden,
    bool enableWikiTranslation = true)
  {
    MapsNodesFullRelationsDto dto;
    if (nodeId > 0)
    {
      dto = await GetNodeAsync(mapId, nodeId, hideHidden, enableWikiTranslation);
      if (!dto.Id.HasValue)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelNode, nodeId);
    }
    else
    {
      dto = await GetRootNodeAsync(mapId, hideHidden);
      if (!dto.Id.HasValue)
        throw new OLabGeneralException($"map {mapId} has no root node");
    }

    return dto;
  }

  /// <summary>
  /// ReadAsync map node with out scoped objects
  /// </summary>
  /// <param name="mapId">map id</param>
  /// <param name="nodeId">node id</param>
  /// <param name="hideHidden">Flag to suppress hidden links</param>
  /// <returns>MapsNodesFullRelationsDto</returns>
  public async Task<MapsNodesFullRelationsDto> GetMapNodeAsync(
    IOLabAuthorization auth,
    uint mapId,
    uint nodeId,
    bool hideHidden = true)
  {
    GetLogger().LogInformation($"{auth.UserContext.UserId}: MapsEndpoint.GetMapNodeAsync");

    var dto = await GetRawNodeAsync(mapId, nodeId, hideHidden);

    // now that we had a real node id, test if user has explicit no access to node.
    if (await auth.HasAccessAsync(IOLabAuthorization.AclBitMaskNoAccess, Utils.Constants.ScopeLevelNode, nodeId))
      throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelNode, nodeId);

    // filter out any destination links the user
    // does not have access to 
    var filteredLinks = new List<MapNodeLinksDto>();
    foreach (var mapNodeLink in dto.MapNodeLinks)
    {
      if (await auth.HasAccessAsync(IOLabAuthorization.AclBitMaskNoAccess, Utils.Constants.ScopeLevelNode, mapNodeLink.DestinationId))
        continue;

      filteredLinks.Add(mapNodeLink);
    }

    // replace original map node links with acl-filtered links
    dto.MapNodeLinks = filteredLinks;

    return dto;
  }

  /// <summary>
  /// Plays specific map node
  /// </summary>
  /// <param name="mapId">map id</param>
  /// <param name="nodeId">node id</param>
  /// <param name="sessionId">session id</param>
  /// <returns>IActionResult</returns>
  public async Task<MapsNodesFullRelationsDto> PostMapNodeAsync(
    IOLabAuthorization auth,
    uint mapId,
    uint nodeId,
    DynamicScopedObjectsDto body)
  {
    GetLogger().LogInformation($"{auth.UserContext.UserId}: MapsEndpoint.GetMapNodeAsync. new play? {body.NewPlay}");

    // test if user has access to map.
    if (!await auth.HasAccessAsync(IOLabAuthorization.AclBitMaskRead, Utils.Constants.ScopeLevelMap, mapId))
      throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

    // dump out original dynamic objects for logging
    body.Dump(GetLogger(), "Original");

    // test for valid dynamic objects
    if (!body.IsValid())
      throw new OLabUnauthorizedException("Object validity check failed");

    var map = await MapsReaderWriter.Instance(GetLogger(), GetDbContext()).GetSingleAsync(mapId);
    if (map == null)
      throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapId);

    var dto = await GetRawNodeAsync(mapId, nodeId, true);

    // now that we had a real node id, test if user has explicit no access to node.
    if (await auth.HasAccessAsync(
      IOLabAuthorization.AclBitMaskNoAccess,
      Utils.Constants.ScopeLevelNode,
      dto.Id.Value))
      throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelNode, dto.Id.Value);

    var mapNodesPhys = await _nodesReaderWriter.GetByMapAsync(mapId);

    // filter out any destination links the user
    // does not have access to 
    var filteredLinks = new List<MapNodeLinksDto>();
    foreach (var mapNodeLink in dto.MapNodeLinks)
    {
      if (await auth.HasAccessAsync(
        IOLabAuthorization.AclBitMaskNoAccess,
        Utils.Constants.ScopeLevelNode,
        mapNodeLink.DestinationId))
        continue;

      // test if the destination node is visit-once
      // AND has been visited previously
      var destinationNodePhys =
        mapNodesPhys.FirstOrDefault(x => x.Id == mapNodeLink.DestinationId);
      if (destinationNodePhys.VisitOnce.HasValue && destinationNodePhys.VisitOnce.Value == 1)
      {
        if (body.NodesVisited.Contains(destinationNodePhys.Id))
          continue;
      }

      filteredLinks.Add(mapNodeLink);
    }

    // replace original map node links with acl-filtered links
    dto.MapNodeLinks = filteredLinks;

    UpdateNodeCounter();

    dto.DynamicObjects = await GetDynamicScopedObjectsTranslatedAsync(auth, mapId, nodeId);

    if (body.IsEmpty() || (dto.TypeId == 1))
      // requested a root node, so return an initial set of dynamic objects
      dto.DynamicObjects = await GetDynamicScopedObjectsRawAsync(
        auth,
        mapId,
        nodeId);
    else
    {
      // apply any node open counter actions
      await ProcessNodeOpenCountersAsync(nodeId, body.Map.Counters);
      dto.DynamicObjects.Map = body.Map;
    }

    var session = OLabSession.CreateInstance(
      GetLogger(),
      GetDbContext(),
      auth.UserContext);

    session.SetMapId(mapId);

    // if browser signals a new play, then start a new session
    if (body.NewPlay)
      session.OnStartSession();

    dto.ContextId = session.GetSessionId();
    session.OnPlayNode(dto);

    // extend the session into the new node
    session.OnExtendSessionEnd(nodeId);

    // save current session state to database
    session.SaveSessionState(dto.Id.Value, dto.DynamicObjects);

    dto.DynamicObjects.RefreshChecksum();

    // dump out the dynamic objects for logging
    dto.DynamicObjects.Dump(GetLogger(), "New");

    // initialize/update the nodes visited
    dto.DynamicObjects.NodesVisited = body.NodesVisited;
    if (!body.NodesVisited.Contains(dto.Id.Value))
      dto.DynamicObjects.NodesVisited.Add(dto.Id.Value);

    return dto;
  }

  /// <summary>
  /// Delete a node from the map
  /// </summary>
  /// <param name="mapId">map id that owns node</param>
  /// <param name="nodeId">node id</param>
  /// <returns>IActionResult</returns>
  public async Task<MapNodesPostResponseDto> DeleteNodeAsync(
    IOLabAuthorization auth,
    uint mapId,
    uint nodeId
  )
  {
    GetLogger().LogInformation($"{auth.UserContext.UserId}: MapsEndpoint.DeleteNodeAsync");

    // test if user has access to map.
    if (!await auth.HasAccessAsync(IOLabAuthorization.AclBitMaskWrite, Utils.Constants.ScopeLevelMap, mapId))
      throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

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

      var responseDto = new MapNodesPostResponseDto
      {
        Id = nodeId
      };

      return responseDto;

    }
    catch (Exception)
    {
      await transaction.RollbackAsync();
      throw;
    }
  }

  /// <summary>
  /// Updates specific map node
  /// </summary>
  /// <param name="mapId">map id</param>
  /// <param name="nodeId">node id</param>
  /// <param name="dto">node data</param>
  /// <returns>IActionResult</returns>
  public async Task<MapNodesPostResponseDto> PutNodeAsync(
    IOLabAuthorization auth,
    uint mapId,
    uint nodeId,
    [FromBody] MapNodesFullDto dto
  )
  {
    GetLogger().LogInformation($"{auth.UserContext.UserId}: MapsEndpoint.PutNodeAsync");

    // test if user has access to map.
    if (!await auth.HasAccessAsync(IOLabAuthorization.AclBitMaskWrite, Utils.Constants.ScopeLevelMap, mapId))
      throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

    using var transaction = GetDbContext().Database.BeginTransaction();

    try
    {
      var builder = new ObjectMapper.MapNodesFullMapper(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider());
      var phys = builder.DtoToPhysical(dto);

      // patch up node size, just in case it's not set properly
      if (phys.Height == 0)
        phys.Height = 440;
      if (phys.Width == 0)
        phys.Width = 300;

      GetDbContext().MapNodes.Update(phys);
      await GetDbContext().SaveChangesAsync();
      await transaction.CommitAsync();

      var responseDto = new MapNodesPostResponseDto
      {
        Id = nodeId
      };

      return responseDto;

    }
    catch (Exception)
    {
      await transaction.RollbackAsync();
      throw;
    }

  }

  /// <summary>
  /// ReadAsync node for map
  /// </summary>
  /// <param name="map">Map object</param>
  /// <returns>MapsNodesFullRelationsDto</returns>
  private async Task<MapsNodesFullRelationsDto> GetRootNodeAsync(uint mapId, bool hideHidden)
  {
    var phys = await GetDbContext().MapNodes
      .FirstOrDefaultAsync(x => x.MapId == mapId && x.TypeId.Value == (int)Model.MapNodes.NodeType.RootNode);

    if (phys == null)
    {
      // if no map node by this point, then the map doesn't have a root node
      // defined so take the first one (by id)        
      phys = await GetDbContext().MapNodes
        .Where(x => x.MapId == mapId)
        .OrderBy(x => x.Id)
        .FirstOrDefaultAsync();

      if (phys == null)
        throw new OLabObjectNotFoundException("MapNodes", mapId);
    }

    return await GetNodeAsync(mapId, phys.Id, hideHidden, true);
  }

  /// <summary>
  /// 
  /// </summary>
  public void UpdateNodeCounter()
  {
    var counter = GetDbContext().SystemCounters.Where(x => x.Name == "nodeCounter").FirstOrDefault();

    var value = counter.ValueAsNumber();

    value++;
    counter.ValueFromNumber(value);

    GetDbContext().SystemCounters.Update(counter);
    GetDbContext().SaveChanges();
  }

}
