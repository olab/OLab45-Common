using Microsoft.EntityFrameworkCore;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Data.BusinessObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints.Player;

public partial class MapsEndpoint : OLabEndpoint
{

  /// <summary>
  /// 
  /// </summary>
  /// <param name="mapId"></param>
  /// <param name="nodeId"></param>
  /// <param name="sinceTime"></param>
  /// <returns></returns>
  public async Task<DynamicScopedObjectsDto> GetDynamicScopedObjectsRawAsync(
    IOLabAuthorization auth,
    uint mapId,
    uint nodeId,
    uint sinceTime = 0)
  {
    GetLogger().LogInformation( $"userid: {auth.UserContext.UserId}: MapsEndpoint.GetDynamicScopedObjectsRawAsync" );

    // test if user has access to map.
    if ( !await auth.HasAccessAsync( IOLabAuthorization.AclBitMaskRead, Utils.Constants.ScopeLevelMap, mapId ) )
      throw new OLabUnauthorizedException( Utils.Constants.ScopeLevelMap, mapId );

    var node = await _nodesReaderWriter.GetMapRootNode( mapId, nodeId );
    return await GetDynamicScopedObjectsAsync( 1, node, sinceTime, false );
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="mapId"></param>
  /// <param name="nodeId"></param>
  /// <param name="sinceTime"></param>
  /// <returns></returns>
  public async Task<DynamicScopedObjectsDto> GetDynamicScopedObjectsTranslatedAsync(
    IOLabAuthorization auth,
    uint mapId,
    uint nodeId,
    uint sinceTime = 0)
  {
    GetLogger().LogInformation( $"userid: {auth.UserContext.UserId}: MapsEndpoint.GetDynamicScopedObjectsTranslatedAsync" );

    // test if user has access to map.
    if ( !await auth.HasAccessAsync( IOLabAuthorization.AclBitMaskRead, Utils.Constants.ScopeLevelMap, mapId ) )
      throw new OLabUnauthorizedException( Utils.Constants.ScopeLevelMap, mapId );

    var node = await _nodesReaderWriter.GetMapRootNode( mapId, nodeId );
    return await GetDynamicScopedObjectsAsync( 1, node, sinceTime, true );
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
    var phys = new DynamicScopedObjects(
      GetLogger(),
      GetDbContext(),
      GetWikiProvider(), serverId, node.MapId, node.Id );
    await phys.GetDynamicScopedObjectsAsync();

    var builder = new ObjectMapper.DynamicScopedObjects(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider(), enableWikiTranslation );
    var dto = builder.PhysicalToDto( phys );

    return dto;
  }

  private async Task<IList<CountersDto>> ProcessNodeOpenCountersAsync(uint nodeId, IList<CountersDto> orgDtoList)
  {
    var newDtoList = new List<CountersDto>();
    var node = await _nodesReaderWriter.GetNodeAsync( nodeId );

    if ( node == null )
      throw new OLabObjectNotFoundException( "MapNodes", nodeId );

    var counterActions = await GetDbContext().SystemCounterActions.Where( x =>
      (x.ImageableId == node.Id) &&
      (x.ImageableType == Utils.Constants.ScopeLevelNode) &&
      (x.OperationType == "open") ).ToListAsync();

    GetLogger().LogInformation( $"Found {counterActions.Count} counterActions records for node {node.Id} " );

    foreach ( var counterAction in counterActions )
    {
      var dto = orgDtoList.FirstOrDefault( x => x.Id == counterAction.CounterId );
      if ( dto == null )
        GetLogger().LogError( $"Enable to lookup counter {counterAction.CounterId} in action {counterAction.Id}" );

      else
      {
        // convert to physical object so we can use the counterActions code
        var phys = new ObjectMapper.CounterMapper(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider() ).DtoToPhysical( dto );

        // test if there's a counter action to apply
        if ( counterAction.ApplyFunctionToCounter( phys ) )
        {
          // remove original counter from list
          orgDtoList.Remove( dto );

          dto = new ObjectMapper.CounterMapper(
            GetLogger(),
            GetDbContext(),
            GetWikiProvider() ).PhysicalToDto( phys );
          GetLogger().LogInformation( $"Updated counter '{dto.Name}' ({dto.Id}) with function '{counterAction.Expression}'. now = {dto.Value}" );

          // add updated counter back to list
          orgDtoList.Add( dto );
        }

        newDtoList.Add( dto );

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
    var counterActions = await GetDbContext().SystemCounterActions.Where( x =>
      (x.ImageableId == node.Id) &&
      (x.ImageableType == Utils.Constants.ScopeLevelNode) &&
      (x.OperationType == "open") ).ToListAsync();

    GetLogger().LogInformation( $"Found {counterActions.Count} counterActions records for node {node.Id} " );

    foreach ( var counterAction in counterActions )
    {
      var phys = physList.FirstOrDefault( x => x.Id == counterAction.CounterId );
      if ( phys == null )
        GetLogger().LogError( $"Enable to lookup counter {counterAction.CounterId} in action {counterAction.Id}" );

      else if ( counterAction.ApplyFunctionToCounter( phys ) )
        GetLogger().LogInformation( $"Updated counter '{phys.Name}' ({phys.Id}) with function '{counterAction.Expression}'. now = {phys.Value}" );
    }

    return physList;
  }
}