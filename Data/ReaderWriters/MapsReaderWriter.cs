using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OLab.Api.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Api.Model.ReaderWriter;

public partial class MapsReaderWriter
{
  private readonly OLabDBContext _context;
  private readonly ILogger _logger;

  public static MapsReaderWriter Instance(ILogger logger, OLabDBContext context)
  {
    return new MapsReaderWriter(logger, context);
  }

  public MapsReaderWriter(ILogger logger, OLabDBContext context)
  {
    _context = context;
    _logger = logger;
  }

  public async Task<Maps> DeleteAsync(uint id)
  {
    _context.Database.BeginTransaction();

    try
    {
      var physMap = await _context
        .Maps
        .Include("MapNodes")
        .FirstOrDefaultAsync(x => x.Id == id);

      if ( ( physMap == null ) || (physMap.Id == 0) )
        return null;

      _context.Maps.Remove(physMap);

      // get all the related nodeIds so we can build requests
      // to delete all map nad node scoped objects in one shot
      var nodeIds = physMap.MapNodes.Select(x => x.Id).ToList();

      var constants = _context.SystemConstants.Where(x => (
        (x.ImageableId == id && x.ImageableType == Constants.ScopeLevelMap) ||
        (nodeIds.Contains(x.ImageableId) && x.ImageableType == Constants.ScopeLevelNode))).ToList();
      _context.SystemConstants.RemoveRange(constants);

      var questions = _context.SystemQuestions.Where(x => (
        (x.ImageableId == id && x.ImageableType == Constants.ScopeLevelMap) ||
        (nodeIds.Contains(x.ImageableId) && x.ImageableType == Constants.ScopeLevelNode))).ToList();
      _context.SystemQuestions.RemoveRange(questions);

      var files = _context.SystemFiles.Where(x => (
        (x.ImageableId == id && x.ImageableType == Constants.ScopeLevelMap) ||
        (nodeIds.Contains(x.ImageableId) && x.ImageableType == Constants.ScopeLevelNode))).ToList();
      _context.SystemFiles.RemoveRange(files);

      var counterActions = _context.SystemCounterActions.Where(x => (
        (x.ImageableId == id && x.ImageableType == Constants.ScopeLevelMap) ||
        (nodeIds.Contains(x.ImageableId) && x.ImageableType == Constants.ScopeLevelNode))).ToList();
      _context.SystemCounterActions.RemoveRange(counterActions);

      var counters = _context.SystemCounters.Where(x => (
        (x.ImageableId == id && x.ImageableType == Constants.ScopeLevelMap) ||
        (nodeIds.Contains(x.ImageableId) && x.ImageableType == Constants.ScopeLevelNode))).ToList();
      _context.SystemCounters.RemoveRange(counters);

      _context.SaveChanges();

      _context.Database.CommitTransaction();

      return physMap;
    }
    catch (Exception)
    {
      _context.Database.RollbackTransaction();
    }

    return null;
  }

  public async Task<Maps> GetSingleAsync(uint id)
  {
    var phys = await _context.Maps.FirstOrDefaultAsync(x => x.Id == id);
    if (phys.Id == 0)
      return null;
    return phys;
  }

  public async Task<Maps> GetSingleWithNodesAsync(uint id)
  {
    var phys = await GetSingleAsync(id);
    if (phys != null)
      _context.Entry(phys).Collection(b => b.MapNodes).Load();

    return phys;
  }

  public async Task<IList<Maps>> GetMultipleAsync(int skip = 0, int take = 0)
  {
    var items = await _context.Maps.Skip(skip).Take(take).OrderBy(x => x.Name).ToListAsync();
    return items;
  }

  public async Task<uint> UpsertAsync(Maps phys, bool save = true)
  {
    if (phys.Id == 0)
      await _context.Maps.AddAsync(phys);
    else
      _context.Maps.Update(phys);

    if (save)
      await _context.SaveChangesAsync();

    return phys.Id;
  }

  /// <summary>
  /// Add template to new map
  /// </summary>
  /// <param name="map">Target map (map be null, meaning create new map)</param>
  /// <param name="templateId">Source template</param>
  /// <returns>Ammended map</returns>
  public async Task<Maps> CreateMapWithTemplateAsync(Maps map, Maps template)
  {
    using var transaction = _context.Database.BeginTransaction();

    try
    {
      map = await CloneMapAsync(map, template);
      await _context.SaveChangesAsync();
      await transaction.CommitAsync();
    }
    catch (Exception)
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
    if (map == null)
    {
      map = Maps.CreateDefault(template);
      map.Id = 0;
      _context.Entry(map).State = EntityState.Added;
      await _context.SaveChangesAsync();

      _logger.LogError($"  New Map {map.Id}");
    }
    else
    {
      // calculate a box containing all the map nodes
      mapBoundingBox.Load(map.MapNodes.ToList());
      points.Clear();
    }

    // calculate a box containing all the template nodes
    templateBoundingBox.Load(template.MapNodes.ToList());

    // calculate the positional differences between the template
    // and the original map
    var transformVector = templateBoundingBox.CalculateTransformTo(mapBoundingBox);

    _logger.LogDebug($"map BB: {mapBoundingBox.Rect}");
    _logger.LogDebug($"template BB: {templateBoundingBox.Rect}");
    _logger.LogDebug($"transform vector: {transformVector}");

    // reassign nodes to target map and add to target map
    foreach (var node in template.MapNodes)
    {
      var oldNodeId = node.Id;
      MapNodes.Reassign(map, node);

      // if template node is a root node AND we are adding to an existing
      // map, then clear the root node flag for this node
      if (!mapBoundingBox.IsEmpty() && (node.TypeId == 1))
        node.TypeId = 2;

      // transform position of node using the transform vector
      var nodeCoord = new PointF((float)node.X.Value, (float)node.Y.Value);
      var newCoord = nodeCoord + transformVector;

      _logger.LogDebug($"transforming: {nodeCoord} -> {newCoord}");

      node.X = newCoord.X;
      node.Y = newCoord.Y;

      _context.Entry(node).State = EntityState.Added;
      await _context.SaveChangesAsync();

      _logger.LogDebug($"  Node {oldNodeId} -> {node.Id}");
      reverseNodeIdMap[oldNodeId] = node.Id;
    }

    map.AppendMapNodes(template);

    _logger.LogDebug($"{reverseNodeIdMap.Count} node ids to be remapped");

    var templateLinks = _context.MapNodeLinks.AsNoTracking().Where(x => x.MapId == template.Id).ToList();

    foreach (var templateLink in templateLinks)
    {
      var oldNodeLinkId = templateLink.Id;
      MapNodeLinks.Reassign(reverseNodeIdMap, map.Id, templateLink);

      _context.Entry(templateLink).State = EntityState.Added;
      await _context.SaveChangesAsync();

      _logger.LogDebug($"  Link {oldNodeLinkId} -> {templateLink.Id}");
    }

    return map;

  }
}
