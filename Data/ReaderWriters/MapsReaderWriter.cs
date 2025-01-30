using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Data.ReaderWriters;

public partial class MapsReaderWriter : ReaderWriter
{
  public static MapsReaderWriter Instance(IOLabLogger logger, OLabDBContext context)
  {
    return new MapsReaderWriter( logger, context );
  }

  public MapsReaderWriter(IOLabLogger logger, OLabDBContext context) : base( logger, context )
  {
  }

  /// <summary>
  /// Get list of map ids
  /// </summary>
  /// <returns>List of map ids</returns>
  public IList<IdName> GetMapIdNames()
  {
    return GetDbContext().Maps
      .Select( x => new IdName() { Id = x.Id, Name = x.Name } ).ToList();
  }

  public async Task<Maps> DeleteAsync(uint id)
  {
    GetDbContext().Database.BeginTransaction();

    try
    {
      var physMap = await GetDbContext()
        .Maps
        .Include( "MapNodes" )
        .FirstOrDefaultAsync( x => x.Id == id );

      if ( (physMap == null) || (physMap.Id == 0) )
        return null;

      GetDbContext().Maps.Remove( physMap );

      // get all the related nodeIds so we can build requests
      // to delete all map nad node scoped objects in one shot
      var nodeIds = physMap.MapNodes.Select( x => x.Id ).ToList();

      var constants = GetDbContext().SystemConstants.Where( x => (
        (x.ImageableId == id && x.ImageableType == Constants.ScopeLevelMap) ||
        (nodeIds.Contains( x.ImageableId ) && x.ImageableType == Constants.ScopeLevelNode)) ).ToList();
      GetDbContext().SystemConstants.RemoveRange( constants );

      var questions = GetDbContext().SystemQuestions.Where( x => (
        (x.ImageableId == id && x.ImageableType == Constants.ScopeLevelMap) ||
        (nodeIds.Contains( x.ImageableId ) && x.ImageableType == Constants.ScopeLevelNode)) ).ToList();
      GetDbContext().SystemQuestions.RemoveRange( questions );

      var files = GetDbContext().SystemFiles.Where( x => (
        (x.ImageableId == id && x.ImageableType == Constants.ScopeLevelMap) ||
        (nodeIds.Contains( x.ImageableId ) && x.ImageableType == Constants.ScopeLevelNode)) ).ToList();
      GetDbContext().SystemFiles.RemoveRange( files );

      var counterActions = GetDbContext().SystemCounterActions.Where( x => (
        (x.ImageableId == id && x.ImageableType == Constants.ScopeLevelMap) ||
        (nodeIds.Contains( x.ImageableId ) && x.ImageableType == Constants.ScopeLevelNode)) ).ToList();
      GetDbContext().SystemCounterActions.RemoveRange( counterActions );

      var counters = GetDbContext().SystemCounters.Where( x => (
        (x.ImageableId == id && x.ImageableType == Constants.ScopeLevelMap) ||
        (nodeIds.Contains( x.ImageableId ) && x.ImageableType == Constants.ScopeLevelNode)) ).ToList();
      GetDbContext().SystemCounters.RemoveRange( counters );

      GetDbContext().SaveChanges();

      GetDbContext().Database.CommitTransaction();

      return physMap;
    }
    catch ( Exception )
    {
      GetDbContext().Database.RollbackTransaction();
    }

    return null;
  }

  public async Task<Maps> GetSingleAsync(uint id)
  {
    var phys = await GetDbContext().Maps
      .FirstOrDefaultAsync( x => x.Id == id );
    if ( phys.Id == 0 )
      return null;
    return phys;
  }

  public async Task<Maps> GetSingleWithGroupRolesAsync(uint id)
  {
    var phys = await GetDbContext().Maps
      .Include( c => c.MapGrouproles ).ThenInclude( d => d.Group )
      .Include( c => c.MapGrouproles ).ThenInclude( d => d.Role )
      .FirstOrDefaultAsync( x => x.Id == id );

    if ( phys.Id == 0 )
      return null;
    return phys;
  }

  public async Task<Maps> GetSingleWithNodesAsync(uint id)
  {
    var phys = await GetSingleAsync( id );
    if ( phys != null )
      GetDbContext().Entry( phys ).Collection( b => b.MapNodes ).Load();

    return phys;
  }

  public async Task<IList<Maps>> GetMultipleAsync(int skip = 0, int take = 0)
  {
    var items = await GetDbContext().Maps.Skip( skip ).Take( take ).OrderBy( x => x.Name ).ToListAsync();
    return items;
  }

  public async Task<uint> UpsertAsync(Maps newMapPhys, bool save = true)
  {
    if ( newMapPhys.Id == 0 )
    {
      await GetDbContext().Maps.AddAsync( newMapPhys );
      if ( save )
        await GetDbContext().SaveChangesAsync();
    }
    else
    {
      var newMapGroupsPhys = new List<MapGrouproles>();
      newMapGroupsPhys.AddRange( newMapPhys.MapGrouproles );

      // load the entity, then detach it so it can be editted
      var tempMapPhys = GetDbContext().Maps.FirstOrDefault( x => x.Id == newMapPhys.Id );
      if ( tempMapPhys != null )
        GetDbContext().Entry( tempMapPhys ).State = EntityState.Detached;

      newMapPhys.MapGrouproles.Clear();
      newMapPhys.MapGrouproles.AddRange( tempMapPhys.MapGrouproles );

      GetDbContext().Maps.Update( newMapPhys );
      if ( save )
        await GetDbContext().SaveChangesAsync();

      await UpdateGroupRolesAsync( newMapPhys.Id, newMapGroupsPhys.ToList(), save );
    }

    return newMapPhys.Id;
  }

  /// <summary>
  /// Add template to new map
  /// </summary>
  /// <param name="map">Target map (map be null, meaning create new map)</param>
  /// <param name="templateId">Source template</param>
  /// <returns>Ammended map</returns>
  public async Task<Maps> CreateMapWithTemplateAsync(Maps map, Maps template)
  {
    using var transaction = GetDbContext().Database.BeginTransaction();

    try
    {
      map = await CloneMapAsync( map, template );
      await GetDbContext().SaveChangesAsync();
      await transaction.CommitAsync();
    }
    catch ( Exception )
    {
      await transaction.RollbackAsync();
      throw;
    }

    return map;

  }

  /// <summary>
  /// Adds template nodes to a map
  /// </summary>
  /// <param name="map">Target map</param>
  /// <param name="template">Source template map</param>
  /// <returns>Modified map</returns>
  private async Task<Maps> CloneMapAsync(Maps map, Maps template)
  {
    var oldMapId = template.Id;

    var reverseNodeIdMap = new Dictionary<uint, uint>();
    var mapBoundingBox = new MapNodeBoundingBox();
    var templateBoundingBox = new MapNodeBoundingBox();
    var points = new List<PointF>();

    // if no map passed in, then create a new one
    // which will be added to the database.  otherwise
    // try and copy the template nodes to the map
    if ( map == null )
    {
      map = Maps.CreateDefault( template );
      map.Id = 0;
      GetDbContext().Entry( map ).State = EntityState.Added;
      await GetDbContext().SaveChangesAsync();

      GetLogger().LogError( $"  New Map {map.Id}" );
    }
    else
    {
      // calculate a box containing all the map nodes
      mapBoundingBox.Load( map.MapNodes.ToList() );
      points.Clear();
    }

    // calculate a box containing all the template nodes
    templateBoundingBox.Load( template.MapNodes.ToList() );

    // calculate the positional differences between the template
    // and the original map
    var transformVector = templateBoundingBox.CalculateTransformTo( mapBoundingBox );

    GetLogger().LogDebug( $"map BB: {mapBoundingBox.Rect}" );
    GetLogger().LogDebug( $"template BB: {templateBoundingBox.Rect}" );
    GetLogger().LogDebug( $"transform vector: {transformVector}" );

    // reassign nodes to target map and add to target map
    foreach ( var node in template.MapNodes )
    {
      var oldNodeId = node.Id;
      MapNodes.Reassign( map, node );

      // if template node is a root node AND we are adding to an existing
      // map, then clear the root node flag for this node
      if ( !mapBoundingBox.IsEmpty() && (node.TypeId == 1) )
        node.TypeId = 2;

      // transform position of node using the transform vector
      var nodeCoord = new PointF( (float)node.X.Value, (float)node.Y.Value );
      var newCoord = nodeCoord + transformVector;

      GetLogger().LogDebug( $"transforming: {nodeCoord} -> {newCoord}" );

      node.X = newCoord.X;
      node.Y = newCoord.Y;

      GetDbContext().Entry( node ).State = EntityState.Added;
      await GetDbContext().SaveChangesAsync();

      GetLogger().LogDebug( $"  Node {oldNodeId} -> {node.Id}" );
      reverseNodeIdMap[ oldNodeId ] = node.Id;
    }

    map.AppendMapNodes( template );

    GetLogger().LogDebug( $"{reverseNodeIdMap.Count} node ids to be remapped" );

    var templateLinks = GetDbContext().MapNodeLinks.AsNoTracking().Where( x => x.MapId == template.Id ).ToList();

    foreach ( var templateLink in templateLinks )
    {
      var oldNodeLinkId = templateLink.Id;
      MapNodeLinks.Reassign( reverseNodeIdMap, map.Id, templateLink );

      GetDbContext().Entry( templateLink ).State = EntityState.Added;
      await GetDbContext().SaveChangesAsync();

      GetLogger().LogDebug( $"  Link {oldNodeLinkId} -> {templateLink.Id}" );
    }

    return map;

  }

  /// <summary>
  /// Update map groups for a map
  /// </summary>
  /// <param name="mapId">Map id</param>
  /// <param name="groupIds">List of map groups</param>
  /// <param name="save">(optional) include commit to database</param>
  /// <returns>New list of map groups</returns>
  public async Task<IList<MapGrouproles>> UpdateGroupRolesAsync(
    uint mapId,
    IList<MapGrouproles> groupRolesPhys,
    bool save = true)
  {
    var mapPhys = await GetSingleWithGroupRolesAsync( mapId );
    mapPhys.MapGrouproles.Clear();

    foreach ( var groupRolePhys in groupRolesPhys )
      mapPhys.MapGrouproles.Add(
        new MapGrouproles(
          groupRolePhys.MapId,
          groupRolePhys.GroupId,
          groupRolePhys.RoleId ) );

    if ( save )
      await GetDbContext().SaveChangesAsync();

    return mapPhys.MapGrouproles.ToList();
  }
}