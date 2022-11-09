using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLabWebAPI.Common.Exceptions;
using OLabWebAPI.Dto;
using OLabWebAPI.Interface;
using OLabWebAPI.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLabWebAPI.Endpoints.Player
{
  public partial class MapsEndpoint : OlabEndpoint
  {

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mapId"></param>
    /// <param name="nodeId"></param>
    /// <param name="sinceTime"></param>
    /// <returns></returns>
    public async Task<DynamicScopedObjectsDto> GetDynamicScopedObjectsRawAsync(IOLabAuthentication auth, uint mapId, uint nodeId, uint sinceTime = 0)
    {
      logger.LogDebug($"DynamicScopedObjectsController.GetDynamicScopedObjectsRawAsync({mapId}, {nodeId}, {sinceTime})");

      // test if user has access to map.
      if (!auth.HasAccess("R", Utils.Constants.ScopeLevelMap, mapId))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

      var node = await GetMapRootNode(mapId, nodeId);
      return await GetDynamicScopedObjectsAsync(1, node, sinceTime, false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mapId"></param>
    /// <param name="nodeId"></param>
    /// <param name="sinceTime"></param>
    /// <returns></returns>
    public async Task<DynamicScopedObjectsDto> GetDynamicScopedObjectsTranslatedAsync(
      IOLabAuthentication auth, 
      uint mapId, 
      uint nodeId, 
      uint sinceTime = 0)
    {
      logger.LogDebug($"DynamicScopedObjectsController.GetDynamicScopedObjectsTranslatedAsync({mapId}, {nodeId}, {sinceTime})");

      // test if user has access to map.
      if (!auth.HasAccess("R", Utils.Constants.ScopeLevelMap, mapId))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

      var node = await GetMapRootNode(mapId, nodeId);
      return await GetDynamicScopedObjectsAsync(1, node, sinceTime, true);
    }

    /// <summary>
    /// Retrieve dynamic scoped objects for current node
    /// </summary>
    /// <param name="serverId">Server id</param>
    /// <param name="node">Current node</param>
    /// <param name="sinceTime">Look for changes since</param>
    /// <param name="enableWikiTranslation"></param>
    /// <returns></returns>
    public async Task<DynamicScopedObjectsDto> GetDynamicScopedObjectsAsync(
      uint serverId,
      Model.MapNodes node,
      uint sinceTime,
      bool enableWikiTranslation)
    {
      var physServer = new Model.ScopedObjects();
      var physNode = new Model.ScopedObjects();
      var physMap = new Model.ScopedObjects();

      physServer.Counters = await GetScopedCountersAsync(Utils.Constants.ScopeLevelServer, serverId);
      physNode.Counters = await GetScopedCountersAsync(Utils.Constants.ScopeLevelNode, node.Id);
      physMap.Counters = await GetScopedCountersAsync(Utils.Constants.ScopeLevelMap, node.MapId);


      await ProcessNodeCounters(node, physMap.Counters);

      var builder = new ObjectMapper.DynamicScopedObjects(logger, enableWikiTranslation);

      var dto = builder.PhysicalToDto(physServer, physMap, physNode);
      return dto;
    }

    /// <summary>
    /// Apply MapNodeCounter expressions to counters
    /// </summary>
    /// <param name="node">Current node</param>
    /// <param name="counters">Raw system (map-level) counters</param>
    /// <returns>void</returns>
    private async Task ProcessNodeCounters(Model.MapNodes node, IList<SystemCounters> counters)
    {
      var counterActions = await context.SystemCounterActions.Where(x =>
        (x.ImageableId == node.Id) &&
        (x.ImageableType == Utils.Constants.ScopeLevelNode) &&
        (x.OperationType == "open")).ToListAsync();

      logger.LogDebug($"Found {counterActions.Count} counterActions records for node {node.Id} ");

      foreach (var counterAction in counterActions)
      {
        var rawCounter = counters.FirstOrDefault(x => x.Id == counterAction.CounterId);
        if (rawCounter == null)
          logger.LogError($"Enable to lookup counter {counterAction.CounterId} in action {counterAction.Id}");
        else if (counterAction.ApplyFunctionToCounter(rawCounter))
          logger.LogDebug($"Updated counter id {rawCounter.Id} with function '{counterAction.Expression}'. now = {rawCounter.ValueAsString()}");
      }

      return;
    }
  }
}