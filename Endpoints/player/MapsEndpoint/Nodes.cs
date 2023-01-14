using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLabWebAPI.Common;
using OLabWebAPI.Common.Exceptions;
using OLabWebAPI.Dto;
using OLabWebAPI.Data.Interface;
using OLabWebAPI.Model;
using OLabWebAPI.Model.ReaderWriter;
using OLabWebAPI.ObjectMapper;
using OLabWebAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLabWebAPI.Endpoints.Player
{
  public partial class MapsEndpoint : OlabEndpoint
  {
    /// <summary>
    /// Plays specific map node
    /// </summary>
    /// <param name="mapId">map id</param>
    /// <param name="nodeId">node id</param>
    /// <returns>IActionResult</returns>
    public async Task<MapsNodesFullRelationsDto> GetMapNodeAsync(
      IOLabAuthentication auth,
      uint mapId,
      uint nodeId)
    {
      logger.LogDebug($"GetMapNodeAsync(uint mapId={mapId}, nodeId={nodeId})");

      // test if user has access to map.
      if (!auth.HasAccess("R", Utils.Constants.ScopeLevelMap, mapId))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

      // test if user has access to node.
      if (!auth.HasAccess("R", Utils.Constants.ScopeLevelMap, nodeId))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelNode, nodeId);

      var map = await MapsReaderWriter.Instance(logger.GetLogger(), dbContext).GetSingleAsync(mapId);
      if (map == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapId);

      MapsNodesFullRelationsDto dto;
      if (nodeId > 0)
      {
        dto = await GetNodeAsync(map, nodeId);
        if (!dto.Id.HasValue)
          throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelNode, nodeId);
      }
      else
      {
        dto = await GetRootNodeAsync(map);
        if (!dto.Id.HasValue)
          throw new OLabGeneralException($"map {map.Id} has no root node");
      }

      // test if end node, meaning we can close the session.  
      if (dto.End.HasValue && dto.End.Value)
        _userContext.Session.OnEndSession(mapId, dto.Id.Value);

      // if root node, then start a new session
      if (dto.TypeId == 1)
      {
        _userContext.Session.OnStartSession(_userContext, mapId);
        dto.ContextId = _userContext.Session.GetSessionId();
        _userContext.Session.SetSessionId(dto.ContextId);
      }

      _userContext.Session.OnPlayNode(mapId, dto.Id.Value);

      UpdateNodeCounter();

      dto.DynamicObjects = await GetDynamicScopedObjectsTranslatedAsync(auth, mapId, nodeId);

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
      logger.LogDebug($"DeleteNodeAsync(uint mapId={mapId}, nodeId={nodeId})");

      // test if user has access to map.
      if (!auth.HasAccess("W", Utils.Constants.ScopeLevelMap, mapId))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

      using var transaction = dbContext.Database.BeginTransaction();

      try
      {
        var links = dbContext.MapNodeLinks.Where(x => (x.NodeId1 == nodeId) || (x.NodeId2 == nodeId)).ToArray();
        logger.LogDebug($"deleting {links.Count()} links");
        dbContext.MapNodeLinks.RemoveRange(links);

        var node = await dbContext.MapNodes.FirstOrDefaultAsync(x => x.Id == nodeId);
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
      logger.LogDebug($"PutNodeAsync(uint mapId={mapId}, nodeId={nodeId})");

      // test if user has access to map.
      if (!auth.HasAccess("W", Utils.Constants.ScopeLevelMap, mapId))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

      using var transaction = dbContext.Database.BeginTransaction();

      try
      {
        var builder = new ObjectMapper.MapNodesFullMapper(logger);
        var phys = builder.DtoToPhysical(dto);

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
    private async Task<MapsNodesFullRelationsDto> GetRootNodeAsync(Model.Maps map)
    {
      var phys = await dbContext.MapNodes
        .FirstOrDefaultAsync(x => x.MapId == map.Id && x.TypeId.Value == (int)Model.MapNodes.NodeType.RootNode);

      if (phys == null)
      {
        // if no map node by this point, then the map doesn't have a root node
        // defined so take the first one (by id)        
        phys = await dbContext.MapNodes.Where(x => x.MapId == map.Id).OrderBy(x => x.Id).FirstOrDefaultAsync();
      }

      return await GetNodeAsync(map, phys.Id);
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdateNodeCounter()
    {
      var counter = dbContext.SystemCounters.Where(x => x.Name == "nodeCounter").FirstOrDefault();

      var value = counter.ValueAsNumber();

      value++;
      counter.ValueFromNumber(value);

      dbContext.SystemCounters.Update(counter);
      dbContext.SaveChanges();
    }

  }
}
