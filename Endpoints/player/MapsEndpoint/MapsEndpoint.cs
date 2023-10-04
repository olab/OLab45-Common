using Dawn;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Model.ReaderWriter;
using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints.Player
{
  public partial class MapsEndpoint : OLabEndpoint
  {
    public MapsEndpoint(
      IOLabLogger logger,
      IOLabConfiguration configuration,
      OLabDBContext context) 
      : base(
          logger, 
          configuration, 
          context)
    {
    }

    public MapsEndpoint(
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
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Model.Maps> GetSimpleAnonymousAsync(uint id)
    {
      var phys = await dbContext.Maps
        .Include(x => x.SystemCounterActions).FirstOrDefaultAsync(x => x.Id == id);
      return phys;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public Maps GetSimple(OLabDBContext context, uint id)
    {
      var phys = context.Maps
        .Include(x => x.SystemCounterActions).FirstOrDefault(x => x.Id == id);
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
      Logger.LogDebug($"{auth.GetUserContext().UserId}: MapsEndpoint.GetAsync");

      var items = new List<Model.Maps>();
      var total = 0;
      var remaining = 0;

      if (!skip.HasValue)
        skip = 0;

      if (take.HasValue && skip.HasValue)
      {
        items = await dbContext.Maps
          .Skip(skip.Value)
          .Take(take.Value)
          .OrderBy(x => x.Name).ToListAsync();
        remaining = total - take.Value - skip.Value;
      }
      else
      {
        items = await dbContext.Maps.OrderBy(x => x.Name).ToListAsync();
      }

      total = items.Count;

      var dtoList = new MapsMapper(Logger).PhysicalToDto(items);

      Logger.LogDebug(string.Format("found {0} maps", dtoList.Count));

      // filter out any maps user does not have access to.
      dtoList = dtoList.Where(x => auth.HasAccess("R", Utils.Constants.ScopeLevelMap, x.Id)).ToList();

      Logger.LogDebug(string.Format("have access to {0} maps", dtoList.Count));

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
      Logger.LogDebug($"{auth.GetUserContext().UserId}: MapsEndpoint.GetAsync");

      var map = await GetMapAsync(id);
      if (map == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, id);

      // test if map set to use ACL for testing access
      //if (map.SecurityId == 3)
      //{
      // test if user has access to map.
      if (!auth.HasAccess("R", Utils.Constants.ScopeLevelMap, id))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, id);
      //}

      var dto = new MapsFullMapper(Logger).PhysicalToDto(map);

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
      Logger.LogDebug($"{auth.GetUserContext().UserId}: MapsEndpoint.PostExtendMapAsync");

      // test if user has access to map.
      if (!auth.HasAccess("W", Utils.Constants.ScopeLevelMap, mapId))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

      var map = await dbContext.Maps
  .AsNoTracking()
  .Include(x => x.MapNodes)
  .FirstOrDefaultAsync(x => x.Id == mapId);
      if (map == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapId);

      var template = await dbContext.Maps
  .AsNoTracking()
  .Include(x => x.MapNodes)
  .FirstOrDefaultAsync(x => x.Id == body.TemplateId);
      if (template == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, body.TemplateId);

      map = await MapsReaderWriter.Instance(Logger.GetLogger(), dbContext)
        .CreateMapWithTemplateAsync(map, template);

      var mapLinks = dbContext.MapNodeLinks.AsNoTracking().Where(x => x.MapId == map.Id).ToList();
      var linksDto = new MapNodeLinksMapper(Logger).PhysicalToDto(mapLinks);

      var mapNodes = dbContext.MapNodes.AsNoTracking().Where(x => x.MapId == map.Id).ToList();
      var nodesDto = new MapNodesFullMapper(Logger).PhysicalToDto(mapNodes);

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
      Logger.LogDebug($"{auth.GetUserContext().UserId}: MapsEndpoint.PostCreateMapAsync");

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
        var templateMap = await dbContext.Maps
          .AsNoTracking()
          .Include(x => x.MapNodes)
          .FirstOrDefaultAsync(x => x.Id == body.TemplateId);

        if (templateMap == null)
          throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, body.TemplateId.Value);

        map = await MapsReaderWriter.Instance(Logger.GetLogger(), dbContext)
          .CreateMapWithTemplateAsync(map, templateMap);
      }

      // set up default ACL for map author against map
      var acl = SecurityUsers.CreateDefaultMapACL(auth.GetUserContext(), map);
      dbContext.SecurityUsers.Add(acl);

      // update map's author
      map.AuthorId = acl.UserId;
      dbContext.Entry(map).State = EntityState.Modified;

      await dbContext.SaveChangesAsync();

      var dto = new MapsFullRelationsMapper(Logger).PhysicalToDto(map);
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
      Logger.LogDebug($"{auth.GetUserContext().UserId}: MapsEndpoint.PutAsync");

      var map = new MapsFullMapper(Logger).DtoToPhysical(mapdto);

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
        var existingMap = GetSimple(dbContext, id);
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
      Logger.LogDebug($"{auth.GetUserContext().UserId}: MapsEndpoint.DeleteAsync");

      // test if user has access to map.
      if (!auth.HasAccess("D", Utils.Constants.ScopeLevelMap, id))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, id);

      var map = GetSimple(dbContext, id);
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
      Logger.LogDebug($"{auth.GetUserContext().UserId}: MapsEndpoint.GetLinksAsync");

      // test if user has access to map.
      if (!auth.HasAccess("R", Utils.Constants.ScopeLevelMap, mapId))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

      var map = GetSimple(dbContext, mapId);
      if (map == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapId);

      var items = await dbContext.MapNodeLinks.Where(x => x.MapId == mapId).ToListAsync();
      Logger.LogDebug(string.Format("found {0} MapNodeLinks", items.Count));

      var dtoList = new MapNodeLinksFullMapper(Logger).PhysicalToDto(items);
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
      Logger.LogDebug($"{auth.GetUserContext().UserId}: MapsEndpoint.GetSessionsAsync");

      // test if user has access to map.
      if (!auth.HasAccess("W", Utils.Constants.ScopeLevelMap, mapId))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

      var map = GetSimple(dbContext, mapId);

      if (map == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapId);

      var userSessions = await dbContext.UserSessions
        .AsNoTracking()
        .Include(x => x.UserSessionTraces)
        .Where(x => x.MapId == mapId)
        .Select(x => new
        {
          uuid = x.Uuid,
          nodesVisited = x.UserSessionTraces.Where(s => s.MapId == mapId).Count(),
          timestamp = x.StartTime,
          user = x.Iss == auth.GetUserContext().Issuer
            ? dbContext.Users.Where(u => u.Id == x.UserId).First()
            : null,
        })
        .ToListAsync();

      var sessions = new List<SessionInfo>();

      foreach (var item in userSessions)
      {
        sessions.Add(new SessionInfo
        {
          uuid = item.uuid,
          Timestamp = DateTimeOffset.FromUnixTimeSeconds((long)item.timestamp).LocalDateTime,
          User = item.user != null
            ? (String.IsNullOrEmpty(item.user.Nickname) ? item.user.Username : item.user.Nickname)
            : null,
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
