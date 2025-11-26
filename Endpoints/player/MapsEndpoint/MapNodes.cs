using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using OLab.Access;
using OLab.Access.Interfaces;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Dto;
using OLab.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints.Player;

public partial class MapsEndpoint : OLabEndpoint
{
  /// <summary>
  /// Plays specific map node
  /// </summary>
  /// <param name="mapId">map id</param>
  /// <param name="nodeId">node id</param>
  /// <param name="sessionId">session id</param>
  /// <returns>IActionResult</returns>
  public async Task<MapsNodesFullRelationsDto> PlayMapNodeAsync(
    IOLabAuthorization auth,
    uint mapId,
    uint nodeId,
    DynamicScopedObjectsDto body)
  {
    // test if user has access to map.
    if ( !await auth.HasAccessAsync( IOLabAuthorization.AclBitMaskRead, Utils.Constants.ScopeLevelMap, mapId ) )
      throw new OLabUnauthorizedException( Utils.Constants.ScopeLevelMap, mapId );

    GetLogger().LogInformation( $"{auth.OLabUser.Id}: MapsEndpoint.PlayMapNodeAsync: map {mapId}, node {nodeId}, new play? {body.NewPlay}" );

    // dump out original dynamic objects for logging
    body.Dump( GetLogger(), "Original" );

    // test for valid dynamic objects
    if ( !body.IsValid() )
      throw new OLabUnauthorizedException( "Object validity check failed" );

    var nodeDto = await GetRawNodeAsync( mapId, nodeId, true );

    // now that we had a real node id (because the call may be asking for node '0', which
    // is the map root node), test if user has access to node.
    if ( !await auth.HasAccessAsync(
      IOLabAuthorization.AclBitMaskRead,
      Utils.Constants.ScopeLevelNode,
      nodeDto.Id.Value ) )
      throw new OLabUnauthorizedException( Utils.Constants.ScopeLevelNode, nodeDto.Id.Value );

    // get all nodes for the map
    var nodesPhys = await _nodesReaderWriter.GetByMapAsync( mapId );

    // filter out any destination links the user does not have access to 
    nodeDto.MapNodeLinks = await ProcessFilteredLinks( auth, body, nodeDto, nodesPhys );

    UpdateNodeCountCounter();

    nodeDto.DynamicObjects = await GetDynamicScopedObjectsTranslatedAsync( auth, mapId, nodeDto.Id.Value );

    //if ( body.IsEmpty() && (nodeDto.TypeId == 1) )
    // requested a root node, so return an initial set of dynamic objects
    if ( body.IsEmpty() )
      nodeDto.DynamicObjects = await GetDynamicScopedObjectsRawAsync(
          auth,
          mapId,
          nodeDto.Id.Value );
    else
    {
      // apply any node open counter actions
      await ProcessOnNodeOpenActions( body, nodeDto );

      // update any server-level counters
      await UpdateServerCounters( body );

      nodeDto.DynamicObjects.Counters = body.Counters;
    }

    await GetDbContext().SaveChangesAsync();

    ApplyNewSession( auth, mapId, nodeDto.Id.Value, body, nodeDto );
    UpdateNodesVisited( body, nodeDto );

    nodeDto.DynamicObjects.RefreshChecksum();

    return nodeDto;
  }

  private static async Task<List<MapNodeLinksDto>> ProcessFilteredLinks(IOLabAuthorization auth, DynamicScopedObjectsDto body, MapsNodesFullRelationsDto nodeDto, IList<MapNodes> nodesPhys)
  {
    var filteredLinks = new List<MapNodeLinksDto>();
    foreach ( var mapNodeLink in nodeDto.MapNodeLinks )
    {
      if ( !await auth.HasAccessAsync(
          IOLabAuthorization.AclBitMaskRead,
          Utils.Constants.ScopeLevelNode,
          mapNodeLink.DestinationId.Value ) )
        continue;

      // test if the destination node is visit-once
      // AND has been visited previously
      var destinationNodePhys =
        nodesPhys.FirstOrDefault( x => x.Id == mapNodeLink.DestinationId );
      if ( destinationNodePhys.VisitOnce.HasValue && destinationNodePhys.VisitOnce.Value == 1 )
      {
        if ( body.NodesVisited.Contains( destinationNodePhys.Id ) )
          continue;
      }

      filteredLinks.Add( mapNodeLink );
    }

    return filteredLinks;
  }

  private async Task ProcessOnNodeOpenActions(DynamicScopedObjectsDto body, MapsNodesFullRelationsDto nodeDto)
  {
    var newCounters = await ProcessNodeOpenCountersAsync(
      nodeDto.Id.Value,
      body.Counters.Where( x => x.ImageableType == Utils.Constants.ScopeLevelMap ).ToList() );

    // update body counter with any that might have just changed
    foreach ( var newCounter in newCounters )
    {
      var targetCounter =
        body.Counters.FirstOrDefault( x => x.Id == newCounter.Id );

      if ( targetCounter != null )
      {
        targetCounter.SetValue( newCounter.Value );
        targetCounter.UpdatedAt = DateTime.UtcNow;
      }
    }
  }

  private async Task UpdateServerCounters(DynamicScopedObjectsDto body)
  {
    var serverCounters
      = body.Counters.Where( x => x.ImageableType == Utils.Constants.ScopeLevelServer ).ToList();

    foreach ( var serverCounter in serverCounters )
    {
      GetLogger().LogInformation( $"Updating server-level counter {serverCounter.Name} = {serverCounter.Value}" );

      var phys = await GetDbContext().SystemCounters
        .FirstOrDefaultAsync( x => x.Id == serverCounter.Id );

      phys.Value = Encoding.ASCII.GetBytes( serverCounter.Value );

      GetDbContext().SystemCounters.Update( phys );
    }
  }

  private void UpdateNodesVisited(DynamicScopedObjectsDto body, MapsNodesFullRelationsDto dto)
  {
    // dump out the dynamic objects for logging
    dto.DynamicObjects.Dump( GetLogger(), "New" );

    // initialize/update the nodes visited
    dto.DynamicObjects.NodesVisited = body.NodesVisited;
    if ( !body.NodesVisited.Contains( dto.Id.Value ) )
      dto.DynamicObjects.NodesVisited.Add( dto.Id.Value );
  }

  private void ApplyNewSession(IOLabAuthorization auth, uint mapId, uint nodeId, DynamicScopedObjectsDto body, MapsNodesFullRelationsDto dto)
  {
    var session = OLabSession.CreateInstance(
      GetLogger(),
      GetDbContext(),
      auth.AuthenticatedContext );

    session.SetMapId( mapId );

    // if browser signals a new play, then start a new session
    if ( body.NewPlay )
      session.OnStartSession();

    dto.ContextId = session.GetSessionId();
    session.OnPlayNode( dto );

    // extend the session into the new node
    session.OnExtendSessionEnd( nodeId );

    // save current session state to database
    session.SaveSessionState( dto.Id.Value, dto.DynamicObjects );
  }

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
    GetLogger().LogInformation( $"MapsEndpoint.GetRawNodeAsync: map {mapId}, node {nodeId}" );

    MapsNodesFullRelationsDto dto;
    if ( nodeId > 0 )
    {
      dto = await GetNodeAsync( mapId, nodeId, hideHidden, enableWikiTranslation );
      if ( !dto.Id.HasValue )
        throw new OLabObjectNotFoundException( Utils.Constants.ScopeLevelNode, nodeId );
    }
    else
    {
      dto = await GetRootNodeAsync( mapId, hideHidden );
      if ( !dto.Id.HasValue )
        throw new OLabGeneralException( $"map {mapId} has no root node" );
    }

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
    // test if user has access to map.
    if ( !await auth.HasAccessAsync( IOLabAuthorization.AclBitMaskWrite, Utils.Constants.ScopeLevelMap, mapId ) )
      throw new OLabUnauthorizedException( Utils.Constants.ScopeLevelMap, mapId );

    GetLogger().LogInformation( $"{auth.OLabUser.Id}: MapsEndpoint.DeleteNodeAsync" );

    nodeId = await _mapNodesReader.DeleteNodeAsync( nodeId );

    var responseDto = new MapNodesPostResponseDto
    {
      Id = nodeId
    };

    return responseDto;
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
    // test if user has access to map.
    if ( !await auth.HasAccessAsync( IOLabAuthorization.AclBitMaskWrite, Utils.Constants.ScopeLevelMap, mapId ) )
      throw new OLabUnauthorizedException( Utils.Constants.ScopeLevelMap, mapId );

    GetLogger().LogInformation( $"{auth.OLabUser.Id}: MapsEndpoint.PutNodeAsync" );

    var newId = await _mapNodesReader.PutNodeAsync( dto, GetWikiProvider() );

    var responseDto = new MapNodesPostResponseDto
    {
      Id = newId
    };

    return responseDto;

  }

  /// <summary>
  /// ReadAsync node for map
  /// </summary>
  /// <param name="map">Map object</param>
  /// <returns>MapsNodesFullRelationsDto</returns>
  private async Task<MapsNodesFullRelationsDto> GetRootNodeAsync(uint mapId, bool hideHidden)
  {
    GetLogger().LogInformation( $"MapsEndpoint.GetRootNodeAsync. mapId {mapId}" );

    var phys = await _mapNodesReader.GetMapRootNode( mapId, 0 );
    var mapper = new ObjectMapper.MapsNodesFullRelationsMapper(
      GetLogger(),
      GetDbContext(),
      GetWikiProvider() );
    var dto = mapper.PhysicalToDto( phys );

    return dto;
  }

  /// <summary>
  /// 
  /// </summary>
  public void UpdateNodeCountCounter()
  {
    var counter = GetDbContext().SystemCounters.Where( x => x.Name == "nodeCounter" ).FirstOrDefault();

    var value = counter.ValueAsNumber();

    value++;
    counter.ValueFromNumber( value );

    GetDbContext().SystemCounters.Update( counter );
    GetDbContext().SaveChanges();
  }

}
