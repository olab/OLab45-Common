using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints.Player
{
  public partial class NodesEndpoint : OlabEndpoint
  {

    public NodesEndpoint(
      OLabLogger logger,
      IOptions<AppSettings> appSettings,
      OLabDBContext context) : base(logger, appSettings, context)
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
      MapNodes phys = context.MapNodes.FirstOrDefault(x => x.Id == id);
      return phys;
    }

    private async Task<MapsNodesFullRelationsDto> GetNodeAsync(uint id, bool enableWikiTranslation)
    {
      MapNodes phys = await GetMapNodeAsync(id);

      var builder = new ObjectMapper.MapsNodesFullRelationsMapper(logger, enableWikiTranslation);
      MapsNodesFullRelationsDto dto = builder.PhysicalToDto(phys);

      return dto;
    }

    /// <summary>
    /// Get full map node, with relations
    /// </summary>
    /// <param name="nodeId">Node id (0, if root node)</param>
    /// <returns>MapsNodesFullRelationsDto response</returns>
    public async Task<MapsNodesFullRelationsDto> GetNodeTranslatedAsync(IOLabAuthentication auth, uint nodeId)
    {
      logger.LogDebug($"{auth.GetUserContext().UserId}: NodesEndpoint.GetNodeTranslatedAsync");
      return await GetNodeAsync(nodeId, true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task PutNodeAsync(IOLabAuthentication auth, uint id, MapNodesFullDto dto)
    {
      logger.LogDebug($"{auth.GetUserContext().UserId}: NodesEndpoint.PutNodeAsync");

      MapNodes phys = await GetMapNodeAsync(id);
      if (phys == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelNode, id);

      // test if user has access to map.
      if (!auth.HasAccess("W", Utils.Constants.ScopeLevelNode, id))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelNode, id);

      var builder = new ObjectMapper.MapNodesFullMapper(logger);
      phys = builder.DtoToPhysical(dto);

      dbContext.Entry(phys).State = EntityState.Modified;

      await dbContext.SaveChangesAsync();

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nodeId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<MapNodeLinksPostResponseDto> PostLinkAsync(
      IOLabAuthentication auth,
      uint nodeId,
      MapNodeLinksPostDataDto data
    )
    {
      logger.LogDebug($"{auth.GetUserContext().UserId}: NodesEndpoint.PostLinkAsync");

      MapNodes node = GetSimple(dbContext, nodeId);
      if (node == null)
        throw new OLabObjectNotFoundException(Constants.ScopeLevelNode, nodeId);

      var phys = MapNodeLinks.CreateDefault();
      phys.NodeId1 = nodeId;
      phys.NodeId2 = data.DestinationId;
      phys.MapId = node.MapId;

      dbContext.MapNodeLinks.Add(phys);
      await dbContext.SaveChangesAsync();
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
      IOLabAuthentication auth,
      uint mapId,
      [FromBody] MapNodesPostDataDto data
    )
    {
      logger.LogDebug($"{auth.GetUserContext().UserId}: NodesEndpoint.PostNodeAsync");

      using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

      try
      {
        var phys = Model.MapNodes.CreateDefault();
        phys.X = data.X;
        phys.Y = data.Y;
        phys.MapId = mapId;

        dbContext.MapNodes.Add(phys);
        await dbContext.SaveChangesAsync();
        logger.LogDebug($"created MapNode id = {phys.Id}");

        var link = new MapNodeLinks
        {
          MapId = mapId,
          NodeId1 = data.SourceId,
          NodeId2 = phys.Id
        };

        dbContext.MapNodeLinks.Add(link);
        await dbContext.SaveChangesAsync();
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
