using Dawn;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;
using OLab.Data;
using OLab.Data.Interface;
using OLab.Data.ReaderWriters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OLab.Api.WikiTag;

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
  public async Task<Model.Maps> GetSimpleAnonymousAsync(uint id)
  {
    var phys = await GetDbContext().Maps
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
    GetLogger().LogInformation($"{auth.UserContext.UserId}: MapsEndpoint.ReadAsync");

    var items = new List<Model.Maps>();
    var total = 0;
    var remaining = 0;

    if (!skip.HasValue)
      skip = 0;

    if (take.HasValue && skip.HasValue)
    {
      items = await GetDbContext().Maps
        .Skip(skip.Value)
        .Take(take.Value)
        .OrderBy(x => x.Name).ToListAsync();
      remaining = total - take.Value - skip.Value;
    }
    else
    {
      items = await GetDbContext().Maps.OrderBy(x => x.Name).ToListAsync();
    }

    total = items.Count;

    var tempDtoList = new MapsMapper(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider()).PhysicalToDto(items);

    GetLogger().LogInformation(string.Format("found {0} maps", tempDtoList.Count));

    // filter out any maps user does not have access to.
    var dtoList = new List<MapsDto>();

    foreach (var tempDtoItem in tempDtoList)
    {
      if (await auth.HasAccessAsync(
        IOLabAuthorization.AclBitMaskRead, 
        Utils.Constants.ScopeLevelMap, 
        tempDtoItem.Id))
          dtoList.Add(tempDtoItem);
    }

    GetLogger().LogInformation(string.Format("have access to {0} maps", dtoList.Count));

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
    GetLogger().LogInformation($"  generating map {mapId} summary");

    var mapPhys = await GetDbContext().Maps
      .Include(map => map.MapNodes)
      .Include(map => map.MapNodeLinks)
      .Include(map => map.MapGroups).ThenInclude( y => y.Group)

      .AsNoTracking()
      .FirstOrDefaultAsync(
        x => x.Id == mapId,
        token);

    if (mapPhys == null)
      throw new OLabObjectNotFoundException("Maps", mapId);

    if (!await auth.HasAccessAsync(IOLabAuthorization.AclBitMaskRead, Utils.Constants.ScopeLevelMap, mapId))
      throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

    var mapDto = new MapsFullRelationsMapper(

        GetLogger(),
        GetDbContext(),
        GetWikiProvider(),
      false
    ).PhysicalToDto(mapPhys);

    var author = $"({auth.UserContext.UserId}): {auth.UserContext.UserName}";

    var createdAt = new DateTime();
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
      CreatedAt = createdAt,
      Abstract = mapDto.Map.Abstract,
      Groups = mapDto.Map.MapGroups.Select( x => x.GroupName).ToList()
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
    GetLogger().LogInformation($"  generating map {mapId} summary");

    var mapPhys = await GetDbContext().Maps
      .Include(map => map.MapNodes)
      .Include(map => map.MapNodeLinks)
      .AsNoTracking()
      .FirstOrDefaultAsync(
        x => x.Id == mapId,
        token);

    if (mapPhys == null)
      throw new OLabObjectNotFoundException("Maps", mapId);

    if (!await auth.HasAccessAsync(IOLabAuthorization.AclBitMaskRead, Utils.Constants.ScopeLevelMap, mapId))
      throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

    var mapDto = new MapsFullRelationsMapper(

        GetLogger(),
        GetDbContext(),
        GetWikiProvider(),
      false
    ).PhysicalToDto(mapPhys);

    var author = $"({auth.UserContext.UserId}): {auth.UserContext.UserName}";

    var createdAt = new DateTime();
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
    if (dto == null)
      dto = new ScopeObjectCount();

    var scopedObjectsPhys = new ScopedObjects(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider());

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
    GetLogger().LogInformation($"{auth.UserContext.UserId}: MapsEndpoint.ReadAsync");

    var readerWriter = MapsReaderWriter.Instance(GetLogger(), GetDbContext());
    var map = await readerWriter.GetSingleWithGroupsAsync(id);
    if (map == null)
      throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, id);

    // only test security if map is not anonymous
    //if (map.SecurityId == Maps.MapSecurityAnonymous)
    //{
    // test if user has access to map.
    if (!await auth.HasAccessAsync(IOLabAuthorization.AclBitMaskRead, Utils.Constants.ScopeLevelMap, id))
      throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, id);
    //}

    var dto = new MapsFullMapper(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider()).PhysicalToDto(map);

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
    GetLogger().LogInformation($"{auth.UserContext.UserId}: MapsEndpoint.PostExtendMapAsync");

    // test if user has access to map.
    if (!await auth.HasAccessAsync(IOLabAuthorization.AclBitMaskWrite, Utils.Constants.ScopeLevelMap, mapId))
      throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

    var map = await GetDbContext().Maps
      .AsNoTracking()
      .Include(x => x.MapNodes)
      .FirstOrDefaultAsync(x => x.Id == mapId);

    if (map == null)
      throw new OLabObjectNotFoundException("Maps", mapId);

    var template = await GetDbContext().Maps
      .AsNoTracking()
      .Include(x => x.MapNodes)
      .FirstOrDefaultAsync(x => x.Id == body.TemplateId);

    if (template == null)
      throw new OLabObjectNotFoundException("Maps", body.TemplateId);

    map = await MapsReaderWriter.Instance(GetLogger(), GetDbContext())
      .CreateMapWithTemplateAsync(map, template);

    var mapLinks = GetDbContext().MapNodeLinks.AsNoTracking().Where(x => x.MapId == map.Id).ToList();
    var linksDto = new MapNodeLinksMapper(GetLogger(), GetDbContext()).PhysicalToDto(mapLinks);

    var mapNodes = GetDbContext().MapNodes.AsNoTracking().Where(x => x.MapId == map.Id).ToList();
    var nodesDto = new MapNodesFullMapper(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider()).PhysicalToDto(mapNodes);

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
    GetLogger().LogInformation($"{auth.UserContext.UserId}: MapsEndpoint.PostCreateMapAsync");

    // test if user has access to map.
    if (!await auth.HasAccessAsync(IOLabAuthorization.AclBitMaskWrite, Utils.Constants.ScopeLevelMap, 0))
      throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, 0);

    Maps map = null;

    // if no templateId passed in, create default map
    if (!body.TemplateId.HasValue)
    {
      map = Maps.CreateDefault();
      GetDbContext().Maps.Add(map);
      await GetDbContext().SaveChangesAsync();
    }
    else
    {
      var template = await GetDbContext().Maps
        .AsNoTracking()
        .Include(x => x.MapNodes)
        .FirstOrDefaultAsync(x => x.Id == body.TemplateId.Value);

      if (template == null)
        throw new OLabObjectNotFoundException("Maps", body.TemplateId.Value);

      map = await MapsReaderWriter.Instance(GetLogger(), GetDbContext())
        .CreateMapWithTemplateAsync(map, template);
    }

    // set up default ACL for map author against map
    var acl = UserAcls.CreateDefault(auth.UserContext, map);
    GetDbContext().UserAcls.Add(acl);

    map.AuthorId = acl.UserId;
    GetDbContext().Entry(map).State = EntityState.Modified;

    await GetDbContext().SaveChangesAsync();

    var dto = new MapsFullRelationsMapper(

        GetLogger(),
        GetDbContext(),
        GetWikiProvider()
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
    GetLogger().LogInformation($"{auth.UserContext.UserId}: MapsEndpoint.PutAsync");

    var newMapPhys = new MapsFullMapper(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider()).DtoToPhysical(mapdto);

    // test if user has access to map.
    if (!await auth.HasAccessAsync(IOLabAuthorization.AclBitMaskWrite, Utils.Constants.ScopeLevelMap, newMapPhys.Id))
      throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, newMapPhys.Id);

    var mapReadWriter = MapsReaderWriter.Instance(GetLogger(), GetDbContext());
    await mapReadWriter.UpsertAsync(newMapPhys);

    //await mapReadWriter.UpdateGroupsAsync(
    //  newMapPhys.Id, 
    //  newMapPhys.MapGroups.Select(x => x.GroupId).ToArray());

    //try
    //{
    //  await GetDbContext().SaveChangesAsync();
    //}
    //catch (DbUpdateConcurrencyException)
    //{
    //  var existingMap = GetSimple(GetDbContext(), id);
    //  if (existingMap == null)
    //    throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, id);
    //}

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
    GetLogger().LogInformation($"{auth.UserContext.UserId}: MapsEndpoint.GetLinksAsync");

    // test if user has access to map.
    if (!await auth.HasAccessAsync(IOLabAuthorization.AclBitMaskRead, Utils.Constants.ScopeLevelMap, mapId))
      throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

    var map = GetSimple(GetDbContext(), mapId);
    if (map == null)
      throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapId);

    var items = await GetDbContext().MapNodeLinks.Where(x => x.MapId == mapId).ToListAsync();
    GetLogger().LogInformation(string.Format("found {0} MapNodeLinks", items.Count));

    var dtoList = new MapNodeLinksFullMapper(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider()).PhysicalToDto(items);
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
    GetLogger().LogInformation($"{auth.UserContext.UserId}: MapsEndpoint.GetSessionsAsync");

    // test if user has access to map.
    if (!await auth.HasAccessAsync(IOLabAuthorization.AclBitMaskWrite, Utils.Constants.ScopeLevelMap, mapId))
      throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

    var map = GetSimple(GetDbContext(), mapId);

    if (map == null)
      throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapId);

    var userSessions = await GetDbContext().UserSessions
      .AsNoTracking()
      .Include(x => x.UserSessiontraces)
      .Where(x => x.MapId == mapId)
      .Select(x => new
      {
        uuid = x.Uuid,
        nodesVisited = x.UserSessiontraces.Where(s => s.MapId == mapId).Count(),
        timestamp = x.StartTime,
        user = x.Iss == auth.UserContext.Issuer
          ? GetDbContext().Users.Where(u => u.Id == x.UserId).First()
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

  public async Task DeleteMapAsync(IOLabAuthorization auth, uint mapId)
  {
    GetLogger().LogInformation($"DeleteMapAsync(uint mapId={mapId})");

    // test if user has access to map.
    if (!await auth.HasAccessAsync(IOLabAuthorization.AclBitMaskWrite, Utils.Constants.ScopeLevelMap, mapId))
      throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

    var map = await MapsReaderWriter.Instance(GetLogger(), GetDbContext()).DeleteAsync(mapId)
      ?? throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapId);

  }

  [HttpOptions]
  public void Options()
  {

  }
}
