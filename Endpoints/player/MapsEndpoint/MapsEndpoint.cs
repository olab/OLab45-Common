using Dawn;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Endpoints.ReaderWriters;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;
using OLab.Data;
using OLab.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints.Player;

public partial class MapsEndpoint : OLabEndpoint
{
  public MapsEndpoint(
    IOLabLogger logger,
    OLabDBContext dbContext) : base(
    logger,
    dbContext)
  {
  }

  public MapsEndpoint(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext dbContext,
    IOLabSession session)
    : base(
        logger,
        configuration,
        dbContext)
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
  public async Task<Maps> GetSimpleAnonymousAsync(uint id)
  {
    var phys = await dbContext.Maps
      .Include(x => x.SystemCounterActions).FirstOrDefaultAsync(x => x.Id == id);

    if (phys == null)
      throw new OLabObjectNotFoundException("Maps", id);

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
  /// ReadAsync a list of maps
  /// </summary>
  /// <param name="take">Max number of records to return</param>
  /// <param name="skip">SKip over a number of records</param>
  /// <returns>IActionResult</returns>
  public async Task<OLabAPIPagedResponse<MapsDto>> GetAsync(
    IOLabAuthorization auth,
    int? take,
    int? skip)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: MapsEndpoint.ReadAsync");

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

    Logger.LogInformation(string.Format("found {0} maps", dtoList.Count));

    // filter out any maps user does not have access to.
    dtoList = dtoList.Where(x => auth.HasAccess("R", Utils.Constants.ScopeLevelMap, x.Id)).ToList();

    Logger.LogInformation(string.Format("have access to {0} maps", dtoList.Count));

    return new OLabAPIPagedResponse<MapsDto> { Data = dtoList, Remaining = remaining, Count = total };
  }

  /// <summary>
  /// Retrieve map status
  /// </summary>
  /// <param name="id">Map Id</param>
  /// <returns>MapStatusDto</returns>
  public async Task<MapStatusDto> GetStatusAbbreviatedAsync(
    IOLabAuthorization auth,
    uint mapId,
    CancellationToken token = default)
  {
    Logger.LogInformation($"  generating map {mapId} summary");

    var mapPhys = await dbContext.Maps
      .Include(map => map.MapNodes)
      .Include(map => map.MapNodeLinks)
      .AsNoTracking()
      .FirstOrDefaultAsync(
        x => x.Id == mapId,
        token);

    if (mapPhys == null)
      throw new OLabObjectNotFoundException("Maps", mapId);

    if (!auth.HasAccess("R", Utils.Constants.ScopeLevelMap, mapId))
      throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

    var mapDto = new MapsFullRelationsMapper(
      Logger,
      _wikiTagProvider as WikiTagProvider,
      false
    ).PhysicalToDto(mapPhys);

    string author = $"({auth.UserContext.UserId}): {auth.UserContext.UserName}";

    DateTime createdAt = new DateTime();
    if (mapPhys.CreatedAt.HasValue)
    {
      createdAt = mapPhys.CreatedAt.Value;
      createdAt = DateTime.SpecifyKind(createdAt, DateTimeKind.Utc);
    }

    var dto = new MapStatusDto
    {
      Id = mapDto.Map.Id.Value,
      Name = mapDto.Map.Name,
      NodeCount = mapDto.MapNodes.Count,
      NodeLinkCount = mapDto.MapNodeLinks.Count,
      Author = author,
      CreatedAt = createdAt
    };

    return dto;

  }

  /// <summary>
  /// Retrieve map status
  /// </summary>
  /// <param name="id">Map Id</param>
  /// <returns>MapStatusDto</returns>
  public async Task<MapStatusDto> GetStatusAsync(
    IOLabAuthorization auth,
    uint mapId,
    CancellationToken token = default)
  {
    Logger.LogInformation($"  generating map {mapId} summary");

    var mapPhys = await dbContext.Maps
      .Include(map => map.MapNodes)
      .Include(map => map.MapNodeLinks)
      .AsNoTracking()
      .FirstOrDefaultAsync(
        x => x.Id == mapId,
        token);

    if (mapPhys == null)
      throw new OLabObjectNotFoundException("Maps", mapId);

    if (!auth.HasAccess("R", Utils.Constants.ScopeLevelMap, mapId))
      throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

    var mapDto = new MapsFullRelationsMapper(
      Logger,
      _wikiTagProvider as WikiTagProvider,
      false
    ).PhysicalToDto(mapPhys);

    string author = $"({auth.UserContext.UserId}): {auth.UserContext.UserName}";

    DateTime createdAt = new DateTime();
    if (mapPhys.CreatedAt.HasValue)
    {
      createdAt = mapPhys.CreatedAt.Value;
      createdAt = DateTime.SpecifyKind(createdAt, DateTimeKind.Utc);
    }

    var dto = new MapStatusDto
    {
      Id = mapDto.Map.Id.Value,
      Name = mapDto.Map.Name,
      NodeCount = mapDto.MapNodes.Count,
      NodeLinkCount = mapDto.MapNodeLinks.Count,
      Author = author,
      CreatedAt = createdAt
    };

    dto.Server = await GetObjectTallyAsync(Utils.Constants.ScopeLevelServer, 1);
    dto.Map = await GetObjectTallyAsync(Utils.Constants.ScopeLevelMap, mapId);

    var nodesCount = new ScopeObjectCount();
    // loop thru the nodes and get node-level scoped objects
    foreach (var nodeDto in mapDto.MapNodes)
      await GetObjectTallyAsync(Utils.Constants.ScopeLevelNode, nodeDto.Id.Value, nodesCount);

    dto.Node = nodesCount;

    dto.UpdateTotal();

    return dto;

  }

  private async Task<ScopeObjectCount> GetObjectTallyAsync(
    string scopeLevel, 
    uint id, 
    ScopeObjectCount dto = null)
  {
    if ( dto == null )
      dto = new ScopeObjectCount();

    var scopedObjectsPhys = new ScopedObjects(
      Logger,
      dbContext);

    // apply scoped objects to the map dto
    await scopedObjectsPhys.AddScopeFromDatabaseAsync(scopeLevel, id);

    dto.Constants += scopedObjectsPhys.ConstantsPhys.Count;
    dto.Files += scopedObjectsPhys.FilesPhys.Count;
    dto.Questions += scopedObjectsPhys.QuestionsPhys.Count;
    dto.Counters += scopedObjectsPhys.CountersPhys.Count;
    dto.CounterActions += scopedObjectsPhys.CounterActionsPhys.Count;

    var questionCount = 0;
    foreach (var questionPhys in scopedObjectsPhys.QuestionsPhys)
      questionCount += questionPhys.SystemQuestionResponses.Count;

    dto.QuestionRepsonses += questionCount;

    return dto;
  }

  /// <summary>
  /// Retrieve a map
  /// </summary>
  /// <param name="id">Map Id</param>
  /// <returns>Map</returns>
  public async Task<MapsFullDto> GetAsync(
    IOLabAuthorization auth,
    uint id)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: MapsEndpoint.ReadAsync");

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
    IOLabAuthorization auth,
    uint mapId,
    ExtendMapRequest body)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: MapsEndpoint.PostExtendMapAsync");

    // test if user has access to map.
    if (!auth.HasAccess("W", Utils.Constants.ScopeLevelMap, mapId))
      throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

    var map = await dbContext.Maps
      .AsNoTracking()
      .Include(x => x.MapNodes)
      .FirstOrDefaultAsync(x => x.Id == mapId);

    if (map == null)
      throw new OLabObjectNotFoundException("Maps", mapId);

    var template = await dbContext.Maps
      .AsNoTracking()
      .Include(x => x.MapNodes)
      .FirstOrDefaultAsync(x => x.Id == body.TemplateId);

    if (template == null)
      throw new OLabObjectNotFoundException("Maps", body.TemplateId);

    map = await MapsReaderWriter.Instance(Logger, dbContext)
      .CreateMapWithTemplateAsync(map, template);

    var mapLinks = dbContext.MapNodeLinks.AsNoTracking().Where(x => x.MapId == map.Id).ToList();
    var linksDto = new MapNodeLinksMapper(Logger).PhysicalToDto(mapLinks);

    var mapNodes = dbContext.MapNodes.AsNoTracking().Where(x => x.MapId == map.Id).ToList();
    var nodesDto = new MapNodesFullMapper(Logger, _wikiTagProvider).PhysicalToDto(mapNodes);

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
    IOLabAuthorization auth,
    CreateMapRequest body)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: MapsEndpoint.PostCreateMapAsync");

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
      var template = await dbContext.Maps
        .AsNoTracking()
        .Include(x => x.MapNodes)
        .FirstOrDefaultAsync(x => x.Id == body.TemplateId.Value);

      if (template == null)
        throw new OLabObjectNotFoundException("Maps", body.TemplateId.Value);

      map = await MapsReaderWriter.Instance(Logger, dbContext)
        .CreateMapWithTemplateAsync(map, template);
    }

    // set up default ACL for map author against map
    var acl = SecurityUsers.CreateDefaultMapACL(auth.UserContext, map);
    dbContext.SecurityUsers.Add(acl);

    // update map's author
    map.AuthorId = acl.UserId;
    dbContext.Entry(map).State = EntityState.Modified;

    await dbContext.SaveChangesAsync();

    var dto = new MapsFullRelationsMapper(
      Logger,
      _wikiTagProvider as WikiTagProvider
    ).PhysicalToDto(map);
    return dto;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="id"></param>
  /// <param name="mapdto"></param>
  /// <returns></returns>
  public async Task PutAsync(
    IOLabAuthorization auth,
    uint id,
    MapsFullDto mapdto)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: MapsEndpoint.PutAsync");

    var mapPhys = new MapsFullMapper(Logger).DtoToPhysical(mapdto);

    // test if user has access to map.
    if (!auth.HasAccess("W", Utils.Constants.ScopeLevelMap, mapPhys.Id))
      throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapPhys.Id);

    if (id != mapPhys.Id)
      throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapPhys.Id);

    dbContext.Entry(mapPhys).State = EntityState.Modified;

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
  /// <param name="mapId"></param>
  /// <returns></returns>
  public async Task<IList<MapNodeLinksFullDto>> GetLinksAsync(
    IOLabAuthorization auth,
    uint mapId)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: MapsEndpoint.GetLinksAsync");

    // test if user has access to map.
    if (!auth.HasAccess("R", Utils.Constants.ScopeLevelMap, mapId))
      throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

    var map = GetSimple(dbContext, mapId);
    if (map == null)
      throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapId);

    var items = await dbContext.MapNodeLinks.Where(x => x.MapId == mapId).ToListAsync();
    Logger.LogInformation(string.Format("found {0} MapNodeLinks", items.Count));

    var dtoList = new MapNodeLinksFullMapper(Logger).PhysicalToDto(items);
    return dtoList;
  }

  /// <summary>
  /// Retrieve all sessions for a given map
  /// </summary>
  /// <param name="mapId"></param>
  /// <returns></returns>
  public async Task<IList<SessionInfo>> GetSessionsAsync(
    IOLabAuthorization auth,
    uint mapId)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: MapsEndpoint.GetSessionsAsync");

    // test if user has access to map.
    if (!auth.HasAccess("W", Utils.Constants.ScopeLevelMap, mapId))
      throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

    var map = GetSimple(dbContext, mapId);

    if (map == null)
      throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapId);

    var userSessions = await dbContext.UserSessions
      .AsNoTracking()
      .Include(x => x.UserSessiontraces)
      .Where(x => x.MapId == mapId)
      .Select(x => new
      {
        uuid = x.Uuid,
        nodesVisited = x.UserSessiontraces.Where(s => s.MapId == mapId).Count(),
        timestamp = x.StartTime,
        user = x.Iss == auth.UserContext.Issuer
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

  public async Task DeleteMapAsync( IOLabAuthorization auth, uint mapId )
  {
    Logger.LogInformation($"DeleteMapAsync(uint mapId={mapId})");

    // test if user has access to map.
    if (!auth.HasAccess("W", Utils.Constants.ScopeLevelMap, mapId))
      throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

    var mapPhys = GetSimple(dbContext, mapId);

    if (mapPhys == null)
      throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapId);

    await MapsReaderWriter.Instance(Logger, dbContext).DeleteAsync(mapPhys);
  }

  [HttpOptions]
  public void Options()
  {

  }
}
