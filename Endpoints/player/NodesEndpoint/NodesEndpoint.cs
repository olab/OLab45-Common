using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLabWebAPI.Model;
using OLabWebAPI.Dto;
using OLabWebAPI.Common.Exceptions;
using OLabWebAPI.Interface;
using OLabWebAPI.Utils;
using System;

namespace OLabWebAPI.Endpoints.Player
{
  public partial class NodesEndpoint : OlabEndpoint
  {

    public NodesEndpoint(
      OLabLogger logger,
      OLabDBContext context) : base(logger, context)
    {
    }

    /// <summary>
    /// Get simple map node, no relations
    /// </summary>
    /// <param name="context">EF DBContext</param>
    /// <param name="id">Node Id</param>
    /// <returns>MapNodes</returns>
    private static Model.MapNodes GetSimple(OLabDBContext context, uint id)
    {
      var phys = context.MapNodes.FirstOrDefault(x => x.Id == id);
      return phys;
    }

    /// <summary>
    /// Get full map node, with relations
    /// </summary>
    /// <param name="nodeId">Node id (0, if root node)</param>
    /// <returns>MapsNodesFullRelationsDto response</returns>
    public async Task<MapsNodesFullRelationsDto> GetNodeTranslatedAsync(uint nodeId)
    {
      return await GetNodeAsync(nodeId, true);
    }

    private async Task<MapsNodesFullRelationsDto> GetNodeAsync(uint id, bool enableWikiTranslation)
    {
      logger.LogDebug($"NodesController.GetNodeAsync(uint nodeId={id}, bool enableWikiTranslation={enableWikiTranslation})");

      var phys = await GetMapNodeAsync(id);

      var builder = new ObjectMapper.MapsNodesFullRelationsMapper(logger, enableWikiTranslation);
      var dto = builder.PhysicalToDto(phys);

      return dto;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task PutNodeAsync(IOLabAuthentication auth, uint id, MapNodesFullDto dto)
    {
      var phys = await GetMapNodeAsync(id);
      if (phys == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelNode, id);      

      // test if user has access to map.
      if (!auth.HasAccess("W", Utils.Constants.ScopeLevelNode, id))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelNode, id);

      var builder = new ObjectMapper.MapNodesFullMapper(logger);
      phys = builder.DtoToPhysical(dto);

      context.Entry(phys).State = EntityState.Modified;

      await context.SaveChangesAsync();

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nodeId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<MapNodeLinksPostResponseDto> PostLinkAsync(
      uint nodeId,
      [FromBody] MapNodeLinksPostDataDto data
    )
    {
      logger.LogDebug($"MapsNodesController.PostAsync(PostLinkAsync(uint nodeId = {nodeId}, uint destinationId = {data.DestinationId})");

      var node = GetSimple( context, nodeId );
      if ( node == null)
        throw new OLabObjectNotFoundException(Constants.ScopeLevelNode, nodeId);

      MapNodeLinks phys = MapNodeLinks.CreateDefault();
      phys.NodeId1 = nodeId;
      phys.NodeId2 = data.DestinationId;
      phys.MapId = node.MapId;

      context.MapNodeLinks.Add(phys);
      await context.SaveChangesAsync();
      logger.LogDebug($"created MapNodeLink id = {phys.Id}");

      var dto = new MapNodeLinksPostResponseDto
      {
        Id = phys.Id
      };

      return dto;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mapId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<MapNodesPostResponseDto> PostNodeAsync(
      uint mapId,
      [FromBody] MapNodesPostDataDto data
    )
    {
      logger.LogDebug($"MapsNodesController.PostAsync(MapNodesFullDto dtoNode)");

      using var transaction = context.Database.BeginTransaction();

      try
      {
        Model.MapNodes phys = Model.MapNodes.CreateDefault();
        phys.X = data.X;
        phys.Y = data.Y;
        phys.MapId = mapId;

        context.MapNodes.Add(phys);
        await context.SaveChangesAsync();
        logger.LogDebug($"created MapNode id = {phys.Id}");

        MapNodeLinks link = new MapNodeLinks
        {
          MapId = mapId,
          NodeId1 = data.SourceId,
          NodeId2 = phys.Id
        };

        context.MapNodeLinks.Add(link);
        await context.SaveChangesAsync();
        logger.LogDebug($"created MapNodeLink id = {link.Id}");

        await transaction.CommitAsync();

        var dto = new MapNodesPostResponseDto
        {
          Id = phys.Id
        };
        dto.Links.Id = link.Id;

        return dto;
      }
      catch (Exception)
      {
        await transaction.RollbackAsync();
        throw;
      }

    }
  }
}
