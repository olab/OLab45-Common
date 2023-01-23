using Microsoft.EntityFrameworkCore;
using OLabWebAPI.Common.Exceptions;
using OLabWebAPI.Data.Interface;
using OLabWebAPI.Dto;
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

      MapNodes node = await GetMapRootNode(mapId, nodeId);
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

      MapNodes node = await GetMapRootNode(mapId, nodeId);
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
      DynamicScopedObjectsDto dto = builder.PhysicalToDto(physServer, physMap, physNode);

      return dto;
    }

    public async Task<IList<CountersDto>> ProcessNodeOpenCountersAsync(uint nodeId, IList<CountersDto> orgDtoList)
    {
      var newDtoList = new List<CountersDto>();
      MapNodes node = await dbContext.MapNodes.FirstOrDefaultAsync(x => x.Id == nodeId);

      List<SystemCounterActions> counterActions = await dbContext.SystemCounterActions.Where(x =>
        (x.ImageableId == node.Id) &&
        (x.ImageableType == Utils.Constants.ScopeLevelNode) &&
        (x.OperationType == "open")).ToListAsync();

      logger.LogDebug($"Found {counterActions.Count} counterActions records for node {node.Id} ");

      foreach (SystemCounterActions counterAction in counterActions)
      {
        CountersDto dto = orgDtoList.FirstOrDefault(x => x.Id == counterAction.CounterId);
        if (dto == null)
          logger.LogError($"Enable to lookup counter {counterAction.CounterId} in action {counterAction.Id}");

        else
        {
          // convert to physical object so we can use the counterActions code
          SystemCounters phys = new ObjectMapper.Counters(logger).DtoToPhysical(dto);

          // test if there's a counter action to apply
          if (counterAction.ApplyFunctionToCounter(phys))
          {
            // remove original counter from list
            orgDtoList.Remove(dto);

            dto = new ObjectMapper.Counters(logger).PhysicalToDto(phys);
            logger.LogDebug($"Updated counter '{dto.Name}' ({dto.Id}) with function '{counterAction.Expression}'. now = {dto.Value}");

            // add updated counter back to list
            orgDtoList.Add(dto);
          }

          newDtoList.Add(dto);

        }

      }

      return newDtoList;
    }

    /// <summary>
    /// Apply MapNodeCounter expressions to orgDtoList
    /// </summary>
    /// <param name="node">Current node</param>
    /// <param name="physList">Raw system (map-level) orgDtoList</param>
    /// <returns>void</returns>
    private async Task<IList<SystemCounters>> ProcessNodeCounters(Model.MapNodes node, IList<SystemCounters> physList)
    {
      List<SystemCounterActions> counterActions = await dbContext.SystemCounterActions.Where(x =>
        (x.ImageableId == node.Id) &&
        (x.ImageableType == Utils.Constants.ScopeLevelNode) &&
        (x.OperationType == "open")).ToListAsync();

      logger.LogDebug($"Found {counterActions.Count} counterActions records for node {node.Id} ");

      foreach (SystemCounterActions counterAction in counterActions)
      {
        SystemCounters phys = physList.FirstOrDefault(x => x.Id == counterAction.CounterId);
        if (phys == null)
          logger.LogError($"Enable to lookup counter {counterAction.CounterId} in action {counterAction.Id}");

        else if (counterAction.ApplyFunctionToCounter(phys))
          logger.LogDebug($"Updated counter '{phys.Name}' ({phys.Id}) with function '{counterAction.Expression}'. now = {phys.Value}");
      }

      return physList;
    }
  }
}