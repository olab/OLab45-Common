using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLabWebAPI.Model;
using OLabWebAPI.Dto;
using OLabWebAPI.ObjectMapper;
using OLabWebAPI.Common;
using OLabWebAPI.Interface;
using OLabWebAPI.Utils;
using System.Text;
using OLabWebAPI.Model.ReaderWriter;
using OLabWebAPI.Common.Exceptions;

namespace OLabWebAPI.Endpoints.Designer
{
  public partial class MapsEndpoint : OlabEndpoint
  {

    public MapsEndpoint(
      OLabLogger logger,
      OLabDBContext context) : base(logger, context)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    private static Model.Maps GetSimple(OLabDBContext context, uint id)
    {
      var phys = context.Maps.Include(x => x.SystemCounterActions).FirstOrDefault(x => x.Id == id);
      return phys;
    }

    /// <summary>
    /// Plays specific map node
    /// </summary>
    /// <param name="mapId">map id</param>
    /// <param name="nodeId">node id</param>
    /// <returns>IActionResult</returns>
    public async Task<MapsNodesFullRelationsDto> GetMapNodeAsync(IOLabAuthentication auth, uint mapId, uint nodeId)
    {
      logger.LogDebug($"GetMapNodeAsync(uint mapId={mapId}, nodeId={nodeId})");

      // test if user has access to map.
      if (!auth.HasAccess("R", Utils.Constants.ScopeLevelMap, mapId))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

      var map = await MapsReaderWriter.Instance(logger.GetLogger(), context).GetSingleAsync(mapId);
      if (map == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapId);

      MapsNodesFullRelationsDto dto;
      // get node with no wikitag translation
      dto = await GetNodeAsync(map, nodeId, false);

      if (!dto.Id.HasValue)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelNode, nodeId);

      return dto;
    }

    /// <summary>
    /// Get non-rendered nodes for a map
    /// </summary>
    /// <param name="mapId">Map id</param>
    /// <returns>IActionResult</returns>
    public async Task<IList<MapNodesFullDto>> GetMapNodesAsync(
      IOLabAuthentication auth, 
      uint mapId)
    {
      logger.LogDebug($"GetMapNodesAsync(uint mapId={mapId})");

      // test if user has access to map.
      if (!auth.HasAccess("R", Utils.Constants.ScopeLevelMap, mapId))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

      var map = await MapsReaderWriter.Instance(logger.GetLogger(), context).GetSingleAsync(mapId);
      if (map == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapId);

      // get node with no wikitag translation
      var dtoList = await GetNodesAsync(map, false);
      return dtoList;
    }

    /// <summary>
    /// Create a new node link
    /// </summary>
    /// <returns>IActionResult</returns>
    public async Task<PostNewLinkResponse> PostMapNodeLinkAsync(
      IOLabAuthentication auth, 
      uint mapId, 
      uint nodeId, 
      PostNewLinkRequest body)
    {
      logger.LogDebug($"PostMapNodeLinkAsync( destinationId = {body.DestinationId})");

      try
      {
        // test if user has access to map.
        if (!auth.HasAccess("W", Utils.Constants.ScopeLevelMap, mapId))
          throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

        var sourceNode = await GetMapNodeAsync(nodeId);
        if (sourceNode == null)
          throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelNode, nodeId);

        var destinationNode = await GetMapNodeAsync(body.DestinationId);
        if (destinationNode == null)
          throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelNode, body.DestinationId);

        var phys = MapNodeLinks.CreateDefault();
        phys.MapId = sourceNode.MapId;
        phys.NodeId1 = sourceNode.Id;
        phys.NodeId2 = destinationNode.Id;
        context.Entry(phys).State = EntityState.Added;

        await context.SaveChangesAsync();

        var dto = new PostNewLinkResponse
        {
          Id = phys.Id
        };

        return dto;
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "PostMapNodeLinkAsync");
        throw;
      }
    }

    /// <summary>
    /// Create a new node
    /// </summary>
    /// <returns>IActionResult</returns>
    public async Task<PostNewNodeResponse> PostMapNodesAsync(
      IOLabAuthentication auth, 
      PostNewNodeRequest body)
    {
      logger.LogDebug($"PostMapNodesAsync(x = {body.X}, y = {body.Y}, sourceId = {body.SourceId})");

      using var transaction = context.Database.BeginTransaction();

      try
      {
        var sourceNode = await GetMapNodeAsync(body.SourceId);
        if (sourceNode == null)
          throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelNode, body.SourceId);

        // test if user has access to map.
        if (!auth.HasAccess("W", Utils.Constants.ScopeLevelMap, sourceNode.MapId))
          throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, sourceNode.MapId);

        var phys = MapNodes.CreateDefault();
        phys.X = body.X;
        phys.Y = body.Y;
        phys.MapId = sourceNode.MapId;
        context.Entry(phys).State = EntityState.Added;

        await context.SaveChangesAsync();

        var link = MapNodeLinks.CreateDefault();
        link.MapId = sourceNode.MapId;
        link.NodeId1 = body.SourceId;
        link.NodeId2 = phys.Id;
        context.Entry(link).State = EntityState.Added;

        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        link.NodeId1Navigation = null;
        link.NodeId2Navigation = null;

        var dto = new PostNewNodeResponse
        {
          Links = link,
          Id = phys.Id
        };

        return dto;
      }
      catch (Exception)
      {
        await transaction.RollbackAsync();
        throw;
      }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<OLabWebAPI.Dto.Designer.ScopedObjectsDto> GetScopedObjectsRawAsync(
      IOLabAuthentication auth, 
      uint id)
    {
      logger.LogDebug($"MapsController.GetScopedObjectsRawAsync(uint id={id})");

      // test if user has access to map.
      if (!auth.HasAccess("R", Utils.Constants.ScopeLevelMap, id))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, id);

      return await GetScopedObjectsAsync(id, false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<OLabWebAPI.Dto.Designer.ScopedObjectsDto> GetScopedObjectsAsync(
      IOLabAuthentication auth, 
      uint id)
    {
      logger.LogDebug($"MapsController.GetScopedObjectsTranslatedAsync(uint id={id})");

      // test if user has access to map.
      // test if user has access to map.
      if (!auth.HasAccess("R", Utils.Constants.ScopeLevelMap, id))
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, id);

      return await GetScopedObjectsAsync(id, true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="enableWikiTranslation"></param>
    /// <returns></returns>
    public async Task<OLabWebAPI.Dto.Designer.ScopedObjectsDto> GetScopedObjectsAsync(
      uint id,
      bool enableWikiTranslation)
    {
      var map = GetSimple(context, id);
      if (map == null)
        return null;

      var phys = await GetScopedObjectsAllAsync(map.Id, Utils.Constants.ScopeLevelMap);
      var physServer = await GetScopedObjectsAllAsync(1, Utils.Constants.ScopeLevelServer);

      phys.Combine(physServer);

      phys.Constants.Add(new SystemConstants
      {
        Id = 0,
        Name = Utils.Constants.ReservedConstantMapId,
        ImageableId = map.Id,
        ImageableType = Utils.Constants.ScopeLevelMap,
        IsSystem = 1,
        Value = Encoding.ASCII.GetBytes(map.Id.ToString())
      });

      phys.Constants.Add(new SystemConstants
      {
        Id = 0,
        Name = Utils.Constants.ReservedConstantMapName,
        ImageableId = map.Id,
        ImageableType = Utils.Constants.ScopeLevelMap,
        IsSystem = 1,
        Value = Encoding.ASCII.GetBytes(map.Name)
      });

      phys.Constants.Add(new SystemConstants
      {
        Id = 0,
        Name = Utils.Constants.ReservedConstantSystemTime,
        ImageableId = 1,
        ImageableType = Utils.Constants.ScopeLevelNode,
        IsSystem = 1,
        Value = Encoding.ASCII.GetBytes(DateTime.UtcNow.ToString() + " UTC")
      });

      var builder = new ObjectMapper.Designer.ScopedObjects(logger, enableWikiTranslation);
      var dto = builder.PhysicalToDto(phys);

      return dto;
    }
  }
}
