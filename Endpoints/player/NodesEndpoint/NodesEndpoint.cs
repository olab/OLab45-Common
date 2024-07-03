using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using System;
using System.Linq;
using System.Threading.Tasks;
using OLab.Api.WikiTag;

namespace OLab.Api.Endpoints.Player;

public partial class NodesEndpoint : OLabEndpoint
{
  public NodesEndpoint(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext context,
    IOLabModuleProvider<IWikiTagModule> wikiTagProvider,
    IOLabModuleProvider<IFileStorageModule> fileStorageProvider)
    : base(
        logger,
        configuration,
        context,
        wikiTagProvider,
        fileStorageProvider)
  {
  }

  /// <summary>
  /// ReadAsync simple map node, no relations
  /// </summary>
  /// <param name="context">EF DBContext</param>
  /// <param name="id">Node Id</param>
  /// <returns>MapNodes</returns>
  private static Model.MapNodes GetSimple(OLabDBContext context, uint id)
  {
    var phys = context.MapNodes.FirstOrDefault(x => x.Id == id);
    return phys;
  }

  private async Task<MapsNodesFullRelationsDto> GetNodeAsync(uint id, bool enableWikiTranslation)
  {
    var phys = await GetMapNodeAsync(id);

    var builder = new ObjectMapper.MapsNodesFullRelationsMapper(
      GetLogger(),
      _wikiTagProvider as WikiTagModuleProvider,
      enableWikiTranslation);
    var dto = builder.PhysicalToDto(phys);

    return dto;
  }

  /// <summary>
  /// ReadAsync full map node, with relations
  /// </summary>
  /// <param name="nodeId">Node id (0, if root node)</param>
  /// <returns>MapsNodesFullRelationsDto response</returns>
  public async Task<MapsNodesFullRelationsDto> GetNodeTranslatedAsync(IOLabAuthorization auth, uint nodeId)
  {
    GetLogger().LogInformation($"{auth.UserContext.UserId}: NodesEndpoint.GetNodeTranslatedAsync");
    return await GetNodeAsync(nodeId, true);
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="id"></param>
  /// <param name="dto"></param>
  /// <returns></returns>
  public async Task PutNodeAsync(IOLabAuthorization auth, uint id, MapNodesFullDto dto)
  {
    GetLogger().LogInformation($"{auth.UserContext.UserId}: NodesEndpoint.PutNodeAsync");

    var phys = await GetMapNodeAsync(id);
    if (phys == null)
      throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelNode, id);

    // test if user has access to map.
    if (!await auth.HasAccessAsync(IOLabAuthorization.AclBitMaskWrite, Utils.Constants.ScopeLevelNode, id))
      throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelNode, id);

    var builder = new ObjectMapper.MapNodesFullMapper(GetLogger());
    phys = builder.DtoToPhysical(dto);

    GetDbContext().Entry(phys).State = EntityState.Modified;

    await GetDbContext().SaveChangesAsync();

  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="nodeId"></param>
  /// <param name="data"></param>
  /// <returns></returns>
  public async Task<MapNodeLinksPostResponseDto> PostLinkAsync(
    IOLabAuthorization auth,
    uint nodeId,
    MapNodeLinksPostDataDto data
  )
  {
    GetLogger().LogInformation($"{auth.UserContext.UserId}: NodesEndpoint.PostLinkAsync");

    var node = GetSimple(GetDbContext(), nodeId);
    if (node == null)
      throw new OLabObjectNotFoundException(Constants.ScopeLevelNode, nodeId);

    var phys = MapNodeLinks.CreateDefault();
    phys.NodeId1 = nodeId;
    phys.NodeId2 = data.DestinationId;
    phys.MapId = node.MapId;

    GetDbContext().MapNodeLinks.Add(phys);
    await GetDbContext().SaveChangesAsync();
    GetLogger().LogInformation($"created MapNodeLink id = {phys.Id}");

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
    IOLabAuthorization auth,
    uint mapId,
    [FromBody] MapNodesPostDataDto data
  )
  {
    GetLogger().LogInformation($"{auth.UserContext.UserId}: NodesEndpoint.PostNodeAsync");

    using var transaction = GetDbContext().Database.BeginTransaction();

    try
    {
      var phys = Model.MapNodes.CreateDefault();
      phys.X = data.X;
      phys.Y = data.Y;
      phys.MapId = mapId;

      GetDbContext().MapNodes.Add(phys);
      await GetDbContext().SaveChangesAsync();
      GetLogger().LogInformation($"created MapNode id = {phys.Id}");

      var link = new MapNodeLinks
      {
        MapId = mapId,
        NodeId1 = data.SourceId,
        NodeId2 = phys.Id
      };

      GetDbContext().MapNodeLinks.Add(link);
      await GetDbContext().SaveChangesAsync();
      GetLogger().LogInformation($"created MapNodeLink id = {link.Id}");

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
