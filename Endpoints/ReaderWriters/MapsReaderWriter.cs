using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OLab.Api.Data.Interface;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace OLab.Api.Endpoints.ReaderWriters;

public partial class MapsReaderWriter : ReaderWriter<Maps>
{
  public static MapsReaderWriter Instance(IOLabLogger logger, OLabDBContext context)
  {
    return new MapsReaderWriter(logger, context);
  }

  public MapsReaderWriter(IOLabLogger logger, OLabDBContext dbContext) : base(logger, dbContext)
  {
  }

  public override async Task<Maps> GetAsync(string nameOrId)
  {
    if (uint.TryParse(nameOrId, out var id))
      return await dbContext.Maps.FirstOrDefaultAsync(e => e.Id == id);

    var myWriter = new StringWriter();

    // Decode any html encoded string.
    HttpUtility.HtmlDecode(nameOrId, myWriter);

    return await dbContext.Maps
      .FirstOrDefaultAsync(e => e.Name == myWriter.ToString());
  }

  public override Task<Maps> EditAsync(Maps phys)
  {
    throw new NotImplementedException();
  }

  public override async Task<Maps> CreateAsync(
    IOLabAuthorization auth,
    Maps phys,
    CancellationToken token = default)
  {
    phys = await base.CreateAsync(auth, phys, token);

    phys.AssignAuthorization(
      dbContext,
      auth.UserContext);

    return phys;
  }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
  public override async Task DeleteAsync(Maps phys)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
  {
    // get all the related nodeIds so we can build requests
    // to delete all map nad node scoped objects in one shot
    var nodeIds = phys.MapNodes.Select(x => x.Id).ToList();

    var constants = dbContext.SystemConstants.Where(x => (
      (x.ImageableId == phys.Id && x.ImageableType == Constants.ScopeLevelMap) ||
      (nodeIds.Contains(x.ImageableId) && x.ImageableType == Constants.ScopeLevelNode))).ToList();
    dbContext.SystemConstants.RemoveRange(constants);

    var questions = dbContext.SystemQuestions.Where(x => (
      (x.ImageableId == phys.Id && x.ImageableType == Constants.ScopeLevelMap) ||
      (nodeIds.Contains(x.ImageableId) && x.ImageableType == Constants.ScopeLevelNode))).ToList();
    dbContext.SystemQuestions.RemoveRange(questions);

    var files = dbContext.SystemFiles.Where(x => (
      (x.ImageableId == phys.Id && x.ImageableType == Constants.ScopeLevelMap) ||
      (nodeIds.Contains(x.ImageableId) && x.ImageableType == Constants.ScopeLevelNode))).ToList();
    dbContext.SystemFiles.RemoveRange(files);

    var counterActions = dbContext.SystemCounterActions.Where(x => (
      (x.ImageableId == phys.Id && x.ImageableType == Constants.ScopeLevelMap) ||
      (nodeIds.Contains(x.ImageableId) && x.ImageableType == Constants.ScopeLevelNode))).ToList();
    dbContext.SystemCounterActions.RemoveRange(counterActions);

    var counters = dbContext.SystemCounters.Where(x => (
      (x.ImageableId == phys.Id && x.ImageableType == Constants.ScopeLevelMap) ||
      (nodeIds.Contains(x.ImageableId) && x.ImageableType == Constants.ScopeLevelNode))).ToList();
    dbContext.SystemCounters.RemoveRange(counters);

    dbContext.Maps.Remove(phys);

  }

  public async Task<Maps> GetAsync(uint id)
  {
    var phys = await dbContext.Maps.FirstOrDefaultAsync(x => x.Id == id);
    if (phys.Id == 0)
      return null;
    return phys;
  }

  public async Task<Maps> GetWithNodesAsync(uint id)
  {
    var phys = await GetAsync(id);
    if (phys != null)
      dbContext.Entry(phys).Collection(b => b.MapNodes).Load();

    return phys;
  }

  public async Task<IList<Maps>> GetPagedAsync(int skip = 0, int take = 0)
  {
    var items = await dbContext.Maps.Skip(skip).Take(take).OrderBy(x => x.Name).ToListAsync();
    return items;
  }

  public async Task<uint> UpsertAsync(Maps phys, bool save = true)
  {
    if (phys.Id == 0)
      await dbContext.Maps.AddAsync(phys);
    else
      dbContext.Maps.Update(phys);

    if (save)
      await dbContext.SaveChangesAsync();

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
    using var transaction = dbContext.Database.BeginTransaction();

    try
    {
      map = await CloneMapAsync(map, template);
      await dbContext.SaveChangesAsync();
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
      dbContext.Entry(map).State = EntityState.Added;
      await dbContext.SaveChangesAsync();

      logger.LogError($"  New Map {map.Id}");
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

    logger.LogDebug($"map BB: {mapBoundingBox.Rect}");
    logger.LogDebug($"template BB: {templateBoundingBox.Rect}");
    logger.LogDebug($"transform vector: {transformVector}");

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

      logger.LogDebug($"transforming: {nodeCoord} -> {newCoord}");

      node.X = newCoord.X;
      node.Y = newCoord.Y;

      dbContext.Entry(node).State = EntityState.Added;
      await dbContext.SaveChangesAsync();

      logger.LogDebug($"  Node {oldNodeId} -> {node.Id}");
      reverseNodeIdMap[oldNodeId] = node.Id;
    }

    map.AppendMapNodes(template);

    logger.LogDebug($"{reverseNodeIdMap.Count} node ids to be remapped");

    var templateLinks = dbContext.MapNodeLinks.AsNoTracking().Where(x => x.MapId == template.Id).ToList();

    foreach (var templateLink in templateLinks)
    {
      var oldNodeLinkId = templateLink.Id;
      MapNodeLinks.Reassign(reverseNodeIdMap, map.Id, templateLink);

      dbContext.Entry(templateLink).State = EntityState.Added;
      await dbContext.SaveChangesAsync();

      logger.LogDebug($"  Link {oldNodeLinkId} -> {templateLink.Id}");
    }

    return map;

  }

}
