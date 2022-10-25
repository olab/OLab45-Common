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
using OLabWebAPI.Dto.Designer;
using OLabWebAPI.Model.ReaderWriter;
using OLabWebAPI.Common.Exceptions;

namespace OLabWebAPI.Endpoints.Player
{
  public partial class MapsEndpoint : OlabEndpoint
  {

    public MapsEndpoint(
      OLabLogger logger,
      OLabDBContext context,
      IOlabAuthentication auth) : base(logger, context, auth)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public Model.Maps GetSimple(OLabDBContext context, uint id)
    {
      var phys = context.Maps.Include(x => x.SystemCounterActions).FirstOrDefault(x => x.Id == id);
      return phys;
    }

    /// <summary>
    /// Get a list of maps
    /// </summary>
    /// <param name="take">Max number of records to return</param>
    /// <param name="skip">SKip over a number of records</param>
    /// <returns>IActionResult</returns>
    public async Task<OLabAPIPagedResponse<MapsDto>> GetAsync(int? take, int? skip)
    {
      logger.LogDebug($"GetAsync([FromQuery] int? take={take}, [FromQuery] int? skip={skip})");

      var items = new List<Model.Maps>();
      var total = 0;
      var remaining = 0;

      if (!skip.HasValue)
        skip = 0;

      if (take.HasValue && skip.HasValue)
      {
        items = await context.Maps.Skip(skip.Value).Take(take.Value).OrderBy(x => x.Name).ToListAsync();
        remaining = total - take.Value - skip.Value;
      }
      else
      {
        items = await context.Maps.OrderBy(x => x.Name).ToListAsync();
      }

      total = items.Count;

      logger.LogDebug(string.Format("found {0} maps", items.Count));

      var dtoList = new MapsMapper(logger).PhysicalToDto(items);

      // filter out any maps user does not have access to.
      dtoList = dtoList.Where(x => auth.HasAccess("R", Utils.Constants.ScopeLevelMap, x.Id)).ToList();
      return new OLabAPIPagedResponse<MapsDto> { Data = dtoList, Remaining = remaining, Count = total };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<MapsFullDto> GetAsync(uint id)
    {
      logger.LogDebug($"GetAsync(uint id={id})");

      // test if user has access to map.
      if (!auth.HasAccess("R", Utils.Constants.ScopeLevelMap, id))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, id);

      var map = GetSimple(context, id);
      if (map == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelNode, id);

      var phys = await GetMapAsync(id);
      var dto = new MapsFullMapper(logger).PhysicalToDto(phys);

      return dto;
    }

    /// <summary>
    /// Append template to an existing map
    /// </summary>
    /// <param name="mapId">Map to add template to</param>
    /// <param name="CreateMapRequest.templateId">Template to add to map</param>
    /// <returns>IActionResult</returns>
    public async Task<ExtendMapResponse> PostExtendMapAsync(uint mapId, ExtendMapRequest body)
    {
      logger.LogDebug($"MapsController.PostExtendMapAsync(mapId = {mapId}, templateId = {body.TemplateId})");

      // test if user has access to map.
      if (!auth.HasAccess("W", Utils.Constants.ScopeLevelMap, mapId))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

      var map = await context.Maps
        .AsNoTracking()
        .Include(x => x.MapNodes)
        .FirstOrDefaultAsync(x => x.Id == mapId);
      if (map == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapId);

      var template = await context.Maps
        .AsNoTracking()
        .Include(x => x.MapNodes)
        .FirstOrDefaultAsync(x => x.Id == body.TemplateId);
      if (template == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, body.TemplateId);

      map = await MapsReaderWriter.Instance(logger.GetLogger(), context)
        .CreateMapWithTemplateAsync(map, template);

      var mapLinks = context.MapNodeLinks.AsNoTracking().Where(x => x.MapId == map.Id).ToList();
      var linksDto = new MapNodeLinksMapper(logger).PhysicalToDto(mapLinks);

      var mapNodes = context.MapNodes.AsNoTracking().Where(x => x.MapId == map.Id).ToList();
      var nodesDto = new MapNodesFullMapper(logger).PhysicalToDto(mapNodes);

      var dto = new ExtendMapResponse
      {
        Nodes = nodesDto,
        Links = linksDto
      };

      return dto;
    }

    /// <summary>
    /// Create new map (using optional template)
    /// </summary>
    /// <param name="body">Create map request body</param>
    /// <returns>IActionResult</returns>
    public async Task<MapsFullRelationsDto> PostCreateMapAsync(CreateMapRequest body)
    {
      logger.LogDebug($"MapsController.PostCreateMapAsync(templateId = {body.TemplateId})");

      // test if user has access to map.
      if (!auth.HasAccess("W", Utils.Constants.ScopeLevelMap, 0))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, 0);

      Maps map = null;

      // if no templateId passed in, create default map
      if (!body.TemplateId.HasValue)
      {
        map = Maps.CreateDefault();
        context.Maps.Add(map);
        await context.SaveChangesAsync();
      }
      else
      {
        var templateMap = await context.Maps
          .AsNoTracking()
          .Include(x => x.MapNodes)
          .FirstOrDefaultAsync(x => x.Id == body.TemplateId);
        if (templateMap == null)
          throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, body.TemplateId.Value);

        map = await MapsReaderWriter.Instance(logger.GetLogger(), context)
          .CreateMapWithTemplateAsync(map, templateMap);
      }

      var dto = new MapsFullRelationsMapper(logger).PhysicalToDto(map);
      return dto;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="mapdto"></param>
    /// <returns></returns>
    public async Task PutAsync(uint id, MapsFullDto mapdto)
    {
      logger.LogDebug($"MapsController.PutAsync(id = {id})");

      var map = new MapsFullMapper(logger).DtoToPhysical(mapdto);

      // test if user has access to map.
      if (!auth.HasAccess("W", Utils.Constants.ScopeLevelMap, map.Id))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, map.Id);

      if (id != map.Id)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, map.Id);

      context.Entry(map).State = EntityState.Modified;

      try
      {
        await context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        var existingMap = GetSimple(context, id);
        if (existingMap == null)
          throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, id);
      }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task DeleteAsync(uint id)
    {
      logger.LogDebug($"DeleteAsync(uint mapId={id})");

      // test if user has access to map.
      if (!auth.HasAccess("D", Utils.Constants.ScopeLevelMap, id))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, id);

      var map = GetSimple(context, id);
      if (map == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, id);

      context.Maps.Remove(map);
      await context.SaveChangesAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mapId"></param>
    /// <returns></returns>
    public async Task<IList<MapNodeLinksFullDto>> GetLinksAsync(uint mapId)
    {
      logger.LogDebug($"GetLinksAsync(uint mapId={mapId})");

      // test if user has access to map.
      if (!auth.HasAccess("R", Utils.Constants.ScopeLevelMap, mapId))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

      var map = GetSimple(context, mapId);
      if (map == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapId);

      var items = await context.MapNodeLinks.Where(x => x.MapId == mapId).ToListAsync();
      logger.LogDebug(string.Format("found {0} MapNodeLinks", items.Count));

      var dtoList = new MapNodeLinksFullMapper(logger).PhysicalToDto(items);
      return dtoList;
    }

    [HttpOptions]
    public void Options()
    {

    }
  }
}
