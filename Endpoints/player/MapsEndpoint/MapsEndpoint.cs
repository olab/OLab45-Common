using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NuGet.Packaging.Signing;
using OLabWebAPI.Common;
using OLabWebAPI.Common.Exceptions;
using OLabWebAPI.Data.Exceptions;
using OLabWebAPI.Data.Interface;
using OLabWebAPI.Dto;
using OLabWebAPI.Model;
using OLabWebAPI.Model.ReaderWriter;
using OLabWebAPI.ObjectMapper;
using OLabWebAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLabWebAPI.Endpoints.Player
{
  public partial class MapsEndpoint : OlabEndpoint
  {

    public MapsEndpoint(
      OLabLogger logger,
      OLabDBContext context) : base(logger, context)
    {
    }

    public MapsEndpoint(
      OLabLogger logger,
      IOptions<AppSettings> appSettings,
      OLabDBContext context) : base(logger, appSettings, context)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Model.Maps> GetSimpleAnonymousAsync(uint id)
    {
      Maps phys = await dbContext.Maps.Include(x => x.SystemCounterActions).FirstOrDefaultAsync(x => x.Id == id);
      return phys;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public Model.Maps GetSimple(OLabDBContext context, uint id)
    {
      Maps phys = context.Maps.Include(x => x.SystemCounterActions).FirstOrDefault(x => x.Id == id);
      return phys;
    }

    /// <summary>
    /// Get a list of maps
    /// </summary>
    /// <param name="take">Max number of records to return</param>
    /// <param name="skip">SKip over a number of records</param>
    /// <returns>IActionResult</returns>
    public async Task<OLabAPIPagedResponse<MapsDto>> GetAsync(
      IOLabAuthentication auth,
      int? take,
      int? skip)
    {
      logger.LogDebug($"{auth.GetUserContext().UserId}: MapsEndpoint.GetAsync");

      var items = new List<Model.Maps>();
      var total = 0;
      var remaining = 0;

      if (!skip.HasValue)
        skip = 0;

      if (take.HasValue && skip.HasValue)
      {
        items = await dbContext.Maps.Skip(skip.Value).Take(take.Value).OrderBy(x => x.Name).ToListAsync();
        remaining = total - take.Value - skip.Value;
      }
      else
      {
        items = await dbContext.Maps.OrderBy(x => x.Name).ToListAsync();
      }

      total = items.Count;

      logger.LogDebug(string.Format("found {0} maps", items.Count));

      IList<MapsDto> dtoList = new MapsMapper(logger).PhysicalToDto(items);

      // filter out any maps user does not have access to.
      dtoList = dtoList.Where(x => auth.HasAccess("R", Utils.Constants.ScopeLevelMap, x.Id)).ToList();
      return new OLabAPIPagedResponse<MapsDto> { Data = dtoList, Remaining = remaining, Count = total };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<MapsFullDto> GetAsync(
      IOLabAuthentication auth,
      uint id)
    {
      logger.LogDebug($"{auth.GetUserContext().UserId}: MapsEndpoint.GetAsync");

      Maps map = await GetMapAsync(id);
      if (map == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, id);

      // test if map set to use ACL for testing access
      //if (map.SecurityId == 3)
      //{
      // test if user has access to map.
      if (!auth.HasAccess("R", Utils.Constants.ScopeLevelMap, id))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, id);
      //}

      MapsFullDto dto = new MapsFullMapper(logger).PhysicalToDto(map);

      return dto;
    }

    /// <summary>
    /// Append template to an existing map
    /// </summary>
    /// <param name="mapId">Map to add template to</param>
    /// <param name="CreateMapRequest.templateId">Template to add to map</param>
    /// <returns>IActionResult</returns>
    public async Task<ExtendMapResponse> PostExtendMapAsync(
      IOLabAuthentication auth,
      uint mapId,
      ExtendMapRequest body)
    {
      logger.LogDebug($"{auth.GetUserContext().UserId}: MapsEndpoint.PostExtendMapAsync");

      // test if user has access to map.
      if (!auth.HasAccess("W", Utils.Constants.ScopeLevelMap, mapId))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

      Maps map = await dbContext.Maps
  .AsNoTracking()
  .Include(x => x.MapNodes)
  .FirstOrDefaultAsync(x => x.Id == mapId);
      if (map == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapId);

      Maps template = await dbContext.Maps
  .AsNoTracking()
  .Include(x => x.MapNodes)
  .FirstOrDefaultAsync(x => x.Id == body.TemplateId);
      if (template == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, body.TemplateId);

      map = await MapsReaderWriter.Instance(logger.GetLogger(), dbContext)
        .CreateMapWithTemplateAsync(map, template);

      var mapLinks = dbContext.MapNodeLinks.AsNoTracking().Where(x => x.MapId == map.Id).ToList();
      IList<MapNodeLinksDto> linksDto = new MapNodeLinksMapper(logger).PhysicalToDto(mapLinks);

      var mapNodes = dbContext.MapNodes.AsNoTracking().Where(x => x.MapId == map.Id).ToList();
      IList<MapNodesFullDto> nodesDto = new MapNodesFullMapper(logger).PhysicalToDto(mapNodes);

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
    public async Task<MapsFullRelationsDto> PostCreateMapAsync(
      IOLabAuthentication auth,
      CreateMapRequest body)
    {
      logger.LogDebug($"{auth.GetUserContext().UserId}: MapsEndpoint.PostCreateMapAsync");

      // test if user has access to map.
      if (!auth.HasAccess("W", Utils.Constants.ScopeLevelMap, 0))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, 0);

      Maps map = null;

      // if no templateId passed in, create default map
      if (!body.TemplateId.HasValue)
      {
        map = Maps.CreateDefault();
        dbContext.Maps.Add(map);
        await dbContext.SaveChangesAsync();
      }
      else
      {
        Maps templateMap = await dbContext.Maps
          .AsNoTracking()
          .Include(x => x.MapNodes)
          .FirstOrDefaultAsync(x => x.Id == body.TemplateId);

        if (templateMap == null)
          throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, body.TemplateId.Value);

        map = await MapsReaderWriter.Instance(logger.GetLogger(), dbContext)
          .CreateMapWithTemplateAsync(map, templateMap);
      }

      // set up default ACL for map author against map
      var acl = SecurityUsers.CreateDefaultMapACL(auth.GetUserContext(), map);
      dbContext.SecurityUsers.Add(acl);
      await dbContext.SaveChangesAsync();

      MapsFullRelationsDto dto = new MapsFullRelationsMapper(logger).PhysicalToDto(map);
      return dto;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="mapdto"></param>
    /// <returns></returns>
    public async Task PutAsync(
      IOLabAuthentication auth,
      uint id,
      MapsFullDto mapdto)
    {
      logger.LogDebug($"{auth.GetUserContext().UserId}: MapsEndpoint.PutAsync");

      Maps map = new MapsFullMapper(logger).DtoToPhysical(mapdto);

      // test if user has access to map.
      if (!auth.HasAccess("W", Utils.Constants.ScopeLevelMap, map.Id))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, map.Id);

      if (id != map.Id)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, map.Id);

      dbContext.Entry(map).State = EntityState.Modified;

      try
      {
        await dbContext.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        Maps existingMap = GetSimple(dbContext, id);
        if (existingMap == null)
          throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, id);
      }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task DeleteAsync(
      IOLabAuthentication auth,
      uint id)
    {
      logger.LogDebug($"{auth.GetUserContext().UserId}: MapsEndpoint.DeleteAsync");

      // test if user has access to map.
      if (!auth.HasAccess("D", Utils.Constants.ScopeLevelMap, id))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, id);

      Maps map = GetSimple(dbContext, id);
      if (map == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, id);

      dbContext.Maps.Remove(map);
      await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mapId"></param>
    /// <returns></returns>
    public async Task<IList<MapNodeLinksFullDto>> GetLinksAsync(
      IOLabAuthentication auth,
      uint mapId)
    {
      logger.LogDebug($"{auth.GetUserContext().UserId}: MapsEndpoint.GetLinksAsync");

      // test if user has access to map.
      if (!auth.HasAccess("R", Utils.Constants.ScopeLevelMap, mapId))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

      Maps map = GetSimple(dbContext, mapId);
      if (map == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapId);

      List<MapNodeLinks> items = await dbContext.MapNodeLinks.Where(x => x.MapId == mapId).ToListAsync();
      logger.LogDebug(string.Format("found {0} MapNodeLinks", items.Count));

      IList<MapNodeLinksFullDto> dtoList = new MapNodeLinksFullMapper(logger).PhysicalToDto(items);
      return dtoList;
    }

    /// <summary>
    /// Retrieve all sessions for a given map
    /// </summary>
    /// <param name="mapId"></param>
    /// <returns></returns>
    public async Task<IList<SessionInfo>> GetSessionsAsync(
      IOLabAuthentication auth,
      uint mapId)
    {
      logger.LogDebug($"{auth.GetUserContext().UserId}: MapsEndpoint.GetSessionsAsync");

      // test if user has access to map.
      if (!auth.HasAccess("W", Utils.Constants.ScopeLevelMap, mapId))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

      Maps map = GetSimple(dbContext, mapId);

      if (map == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapId);

      var query = from session in dbContext.UserSessions
                  join user in dbContext.Users on session.UserId equals user.Id
                    into u from user in u.DefaultIfEmpty()
                  join sessionTrace in dbContext.UserSessionTraces on
                    new
                    {
                      k1 = session.Id,
                      k2 = session.MapId
                    } equals new
                    {
                      k1 = sessionTrace.SessionId,
                      k2 = sessionTrace.MapId
                    }
                    into st from sessionTrace in st.DefaultIfEmpty()
                  where (session.MapId == mapId && session.UserId > 0)
                  select new
                  {
                    user,
                    session,
                    sessionTrace
                  } into joined
                  group joined by new { joined.session.Id } into g
                  select new
                  {
                    uuid = g.Select(j => j.session.Uuid).First(),
                    nodesVisited = g.Select(j => j.sessionTrace).Where(n => n != null).ToList().Count,
                    timestamp = g.Select(j => j.session.StartTime).First(),
                    user = g.Select(j => j.user).First(),
                  };

      var sessions = new List<SessionInfo>();

      foreach (var item in query)
      {
        sessions.Add(new SessionInfo
        {
          uuid = item.uuid,
          Timestamp = DateTimeOffset.FromUnixTimeSeconds((long) item.timestamp).LocalDateTime,
          User = String.IsNullOrEmpty(item.user.Nickname) ? item.user.Username : item.user.Nickname,
          NodesVisited = uint.Parse(item.nodesVisited.ToString()),
        });
      }

      return sessions;
    }

    [HttpOptions]
    public void Options()
    {

    }
  }
}
