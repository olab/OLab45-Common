using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Model.ReaderWriter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints.Player
{
  public partial class MapsEndpoint : OlabEndpoint
  {
    /// <summary>
    /// Gets a node, with no security
    /// </summary>
    /// <param name="mapId">Map Id</param>
    /// <param name="nodeId">Node Id</param>
    /// <param name="hideHidden">Flag to hide hidden links</param>
    /// <returns>MapsNodesFullRelationsDto</returns>
    /// <exception cref="OLabObjectNotFoundException"></exception>
    /// <exception cref="OLabGeneralException"></exception>
    public async Task<MapsNodesFullRelationsDto> GetRawNodeAsync(
      uint mapId,
      uint nodeId,
      bool hideHidden)
    {
      MapsNodesFullRelationsDto dto;
      if (nodeId > 0)
      {
        dto = await GetNodeAsync(mapId, nodeId, hideHidden, true);
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
    /// Get map node with out scoped objects
    /// </summary>
    /// <param name="mapId">map id</param>
    /// <param name="nodeId">node id</param>
    /// <param name="hideHidden">Flag to suppress hidden links</param>
    /// <returns>MapsNodesFullRelationsDto</returns>
    public async Task<MapsNodesFullRelationsDto> GetMapNodeAsync(
      IOLabAuthentication auth,
      uint mapId,
      uint nodeId,
      bool hideHidden = true)
    {
      logger.LogDebug($"{auth.GetUserContext().UserId}: MapsEndpoint.GetMapNodeAsync");

      MapsNodesFullRelationsDto dto = await GetRawNodeAsync(mapId, nodeId, hideHidden);

      // now that we had a real node id, test if user has explicit no access to node.
      if (auth.HasAccess("-", Utils.Constants.ScopeLevelNode, nodeId))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelNode, nodeId);

      // filter out any destination links the user
      // does not have access to 
      var filteredLinks = new List<MapNodeLinksDto>();
      foreach (MapNodeLinksDto mapNodeLink in dto.MapNodeLinks)
      {
        if (auth.HasAccess("-", Utils.Constants.ScopeLevelNode, mapNodeLink.DestinationId))
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
    public async Task<MapsNodesFullRelationsDto> GetMapNodeAsync(
      IOLabAuthentication auth,
      uint mapId,
      uint nodeId,
      DynamicScopedObjectsDto body)
    {
      logger.LogDebug($"{auth.GetUserContext().UserId}: MapsEndpoint.GetMapNodeAsync");

      // test if user has access to map.
      if (!auth.HasAccess("R", Utils.Constants.ScopeLevelMap, mapId))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

      // dump out original dynamic objects for logging
      body.Dump(logger, "Original");

      // test for valid dynamic objects
      if (!body.IsValid())
        throw new OLabUnauthorizedException("Object validity check failed");

      Maps map = await MapsReaderWriter.Instance(logger.GetLogger(), dbContext).GetSingleAsync(mapId);
      if (map == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapId);

      MapsNodesFullRelationsDto dto = await GetRawNodeAsync(mapId, nodeId, true);

      // now that we had a real node id, test if user has explicit no access to node.
      if (auth.HasAccess("-", Utils.Constants.ScopeLevelNode, dto.Id.Value))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelNode, dto.Id.Value);

      // filter out any destination links the user
      // does not have access to 
      var filteredLinks = new List<MapNodeLinksDto>();
      foreach (MapNodeLinksDto mapNodeLink in dto.MapNodeLinks)
      {
        if (auth.HasAccess("-", Utils.Constants.ScopeLevelNode, mapNodeLink.DestinationId))
          continue;

        filteredLinks.Add(mapNodeLink);
      }

      // replace original map node links with acl-filtered links
      dto.MapNodeLinks = filteredLinks;

      // if browser signals a new play, then start a new session
      if (body.NewPlay)
      {
        _userContext.Session.OnStartSession(_userContext, mapId);
        dto.ContextId = _userContext.Session.GetSessionId();
        _userContext.Session.SetSessionId(dto.ContextId);
      }

      _userContext.Session.OnPlayNode(mapId, dto.Id.Value);

      // extend the session into the new node
      _userContext.Session.OnExtendSession(mapId, nodeId);

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

      // save current session state to database
      _userContext.Session.SaveSessionState(mapId, dto.Id.Value, dto.DynamicObjects);

      dto.DynamicObjects.RefreshChecksum();

      // dump out the dynamic objects for logging
      dto.DynamicObjects.Dump(logger, "New");

      return dto;
    }

    /// <summary>
    /// Delete a node from the map
    /// </summary>
    /// <param name="mapId">map id that owns node</param>
    /// <param name="nodeId">node id</param>
    /// <returns>IActionResult</returns>
    public async Task<MapNodesPostResponseDto> DeleteNodeAsync(
      IOLabAuthentication auth,
      uint mapId,
      uint nodeId
    )
    {
      logger.LogDebug($"{auth.GetUserContext().UserId}: MapsEndpoint.DeleteNodeAsync");

      // test if user has access to map.
      if (!auth.HasAccess("W", Utils.Constants.ScopeLevelMap, mapId))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

      using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

      try
      {
        MapNodeLinks[] links = dbContext.MapNodeLinks.Where(x => (x.NodeId1 == nodeId) || (x.NodeId2 == nodeId)).ToArray();
        logger.LogDebug($"deleting {links.Count()} links");
        dbContext.MapNodeLinks.RemoveRange(links);

        MapNodes node = await dbContext.MapNodes.FirstOrDefaultAsync(x => x.Id == nodeId);
        dbContext.MapNodes.Remove(node);
        logger.LogDebug($"deleting node id: {node.Id}");

        await dbContext.SaveChangesAsync();

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
      IOLabAuthentication auth,
      uint mapId,
      uint nodeId,
      [FromBody] MapNodesFullDto dto
    )
    {
      logger.LogDebug($"{auth.GetUserContext().UserId}: MapsEndpoint.PutNodeAsync");

      // test if user has access to map.
      if (!auth.HasAccess("W", Utils.Constants.ScopeLevelMap, mapId))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

      using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

      try
      {
        var builder = new ObjectMapper.MapNodesFullMapper(logger);
        MapNodes phys = builder.DtoToPhysical(dto);

        // patch up node size, just in case it's not set properly
        if (phys.Height == 0) phys.Height = 440;
        if (phys.Width == 0) phys.Width = 300;

        dbContext.MapNodes.Update(phys);
        await dbContext.SaveChangesAsync();
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
    /// Get node for map
    /// </summary>
    /// <param name="map">Map object</param>
    /// <returns>MapsNodesFullRelationsDto</returns>
    private async Task<MapsNodesFullRelationsDto> GetRootNodeAsync(uint mapId, bool hideHidden)
    {
      MapNodes phys = await dbContext.MapNodes
        .FirstOrDefaultAsync(x => x.MapId == mapId && x.TypeId.Value == (int)Model.MapNodes.NodeType.RootNode);

      if (phys == null)
      {
        // if no map node by this point, then the map doesn't have a root node
        // defined so take the first one (by id)        
        phys = await dbContext.MapNodes.Where(x => x.MapId == mapId).OrderBy(x => x.Id).FirstOrDefaultAsync();
      }

      return await GetNodeAsync(mapId, phys.Id, hideHidden, true);
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdateNodeCounter()
    {
      SystemCounters counter = dbContext.SystemCounters.Where(x => x.Name == "nodeCounter").FirstOrDefault();

      var value = counter.ValueAsNumber();

      value++;
      counter.ValueFromNumber(value);

      dbContext.SystemCounters.Update(counter);
      dbContext.SaveChanges();
    }

    public void SaveSessionState(uint mapId, uint nodeId, DynamicScopedObjectsDto dynamicObjects)
    {
      _userContext.Session.SaveSessionState(mapId, nodeId, dynamicObjects);
    }

  }
}
