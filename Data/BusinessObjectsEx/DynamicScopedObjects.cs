using Microsoft.EntityFrameworkCore;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Api.WikiTag;
using OLab.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Data.BusinessObjects;

public class DynamicScopedObjects
{
  protected IOLabLogger _logger;
  protected IOLabLogger GetLogger() { return _logger; }

  private readonly OLabDBContext _dbContext;
  protected OLabDBContext GetDbContext() { return _dbContext; }

  protected IOLabModuleProvider<IWikiTagModule> _wikiTagModules = null;
  public WikiTagModuleProvider GetWikiProvider() { return _wikiTagModules as WikiTagModuleProvider; }

  private readonly uint serverId;
  private readonly uint mapId;
  private readonly uint nodeId;

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
    IOLabModuleProvider<IWikiTagModule> wikiTagModules,
    uint serverId,
    uint mapId,
    uint nodeId)
  {
    _logger = logger;
    _dbContext = dbContext;
    _wikiTagModules = wikiTagModules;

    this.serverId = serverId;
    this.mapId = mapId;
    this.nodeId = nodeId;

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
    var phys = new ScopedObjects( GetLogger(), GetDbContext(), GetWikiProvider() );

    var scopedObjects = await phys.AddScopeFromDatabaseAsync( Constants.ScopeLevelServer, serverId );
    ServerCounters = scopedObjects.CountersPhys;

    scopedObjects = await phys.AddScopeFromDatabaseAsync( Constants.ScopeLevelMap, mapId );
    MapCounters = scopedObjects.CountersPhys;

    scopedObjects = await phys.AddScopeFromDatabaseAsync( Constants.ScopeLevelNode, nodeId );
    NodeCounters = scopedObjects.CountersPhys;

    await ProcessNodeCounters( MapCounters );
  }

  /// <summary>
  /// Apply MapNodeCounter expressions to orgDtoList
  /// </summary>
  /// <param name="node">Current node</param>
  /// <param name="physList">Raw system (map-level) orgDtoList</param>
  /// <returns>void</returns>
  private async Task<IList<SystemCounters>> ProcessNodeCounters(IList<SystemCounters> physList)
  {
    var counterActions = await GetDbContext().SystemCounterActions.Where( x =>
      (x.ImageableId == nodeId) &&
      (x.ImageableType == Constants.ScopeLevelNode) &&
      (x.OperationType == "open") ).ToListAsync();

    GetLogger().LogDebug( $"Found {counterActions.Count} counterActions records for node {nodeId} " );

    foreach ( var counterAction in counterActions )
    {
      var phys = physList.FirstOrDefault( x => x.Id == counterAction.CounterId );
      if ( phys == null )
        GetLogger().LogError( $"Enable to lookup counter {counterAction.CounterId} in action {counterAction.Id}" );

      else if ( counterAction.ApplyFunctionToCounter( phys ) )
        GetLogger().LogDebug( $"Updated counter '{phys.Name}' ({phys.Id}) with function '{counterAction.Expression}'. now = {phys.Value}" );
    }

    return physList;
  }

}
