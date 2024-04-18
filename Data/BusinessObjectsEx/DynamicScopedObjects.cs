using Microsoft.EntityFrameworkCore;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Data.BusinessObjects;

public class DynamicScopedObjects
{
  private readonly IOLabLogger Logger;
  private readonly OLabDBContext dbContext;
  private readonly uint serverId;
  private readonly uint mapId;
  private readonly uint nodeId;
  private readonly IFileStorageModule fileStorageModule;

  public List<SystemCounters> ServerCounters { get; set; }
  public List<SystemCounters> MapCounters { get; set; }
  public List<SystemCounters> NodeCounters { get; set; }

  public DynamicScopedObjects()
  {
    ServerCounters = new List<SystemCounters>();
    MapCounters = new List<SystemCounters>();
    NodeCounters = new List<SystemCounters>();
  }

  public DynamicScopedObjects(
    IOLabLogger logger,
    OLabDBContext dbContext,
    IFileStorageModule fileStorageModule,
    uint serverId,
    uint mapId,
    uint nodeId)
  {
    Logger = logger;
    this.dbContext = dbContext;
    this.serverId = serverId;
    this.mapId = mapId;
    this.nodeId = nodeId;
    this.fileStorageModule = fileStorageModule;
    ServerCounters = new List<SystemCounters>();
    MapCounters = new List<SystemCounters>();
    NodeCounters = new List<SystemCounters>();
  }

  /// <summary>
  /// Retrieve dynamic scoped objects for current node
  /// </summary>
  /// <returns></returns>
  public async Task GetDynamicScopedObjectsAsync()
  {
    var phys = new ScopedObjects(Logger, dbContext, fileStorageModule);

    await phys.AddScopeFromDatabaseAsync(Constants.ScopeLevelServer, serverId);
    await phys.AddScopeFromDatabaseAsync(Constants.ScopeLevelMap, mapId);
    await phys.AddScopeFromDatabaseAsync(Constants.ScopeLevelNode, nodeId);

    ServerCounters = phys.CountersPhys.Where( x => x.ImageableType == Constants.ScopeLevelServer ).ToList();
    NodeCounters = phys.CountersPhys.Where(x => x.ImageableType == Constants.ScopeLevelNode).ToList();
    MapCounters = phys.CountersPhys.Where(x => x.ImageableType == Constants.ScopeLevelMap).ToList();

    await ProcessNodeCounters(MapCounters);
  }

  /// <summary>
  /// Apply MapNodeCounter expressions to orgDtoList
  /// </summary>
  /// <param name="node">Current node</param>
  /// <param name="physList">Raw system (map-level) orgDtoList</param>
  /// <returns>void</returns>
  private async Task<IList<SystemCounters>> ProcessNodeCounters(IList<SystemCounters> physList)
  {
    var counterActions = await dbContext.SystemCounterActions.Where(x =>
      (x.ImageableId == nodeId) &&
      (x.ImageableType == Api.Utils.Constants.ScopeLevelNode) &&
      (x.OperationType == "open")).ToListAsync();

    Logger.LogDebug($"Found {counterActions.Count} counterActions records for node {nodeId} ");

    foreach (var counterAction in counterActions)
    {
      var phys = physList.FirstOrDefault(x => x.Id == counterAction.CounterId);
      if (phys == null)
        Logger.LogError($"Enable to lookup counter {counterAction.CounterId} in action {counterAction.Id}");

      else if (counterAction.ApplyFunctionToCounter(phys))
        Logger.LogDebug($"Updated counter '{phys.Name}' ({phys.Id}) with function '{counterAction.Expression}'. now = {phys.Value}");
    }

    return physList;
  }

}
