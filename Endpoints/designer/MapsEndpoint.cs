using Microsoft.EntityFrameworkCore;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Model.ReaderWriter;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints.Designer
{
  public partial class MapsEndpoint : OLabEndpoint
  {
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
    public async Task<MapsNodesFullRelationsDto> GetMapNodeAsync(IOLabAuthorization auth, uint mapId, uint nodeId)
    {
      Logger.LogDebug($"GetMapNodeAsync(uint mapId={mapId}, nodeId={nodeId})");

      // test if user has access to map.
      if (!auth.HasAccess("R", Utils.Constants.ScopeLevelMap, mapId))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

      var map = await MapsReaderWriter.Instance(Logger.GetLogger(), dbContext).GetSingleAsync(mapId)
        ?? throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapId);

      MapsNodesFullRelationsDto dto;
      // get node with no wikitag translation
      dto = await GetNodeAsync(map.Id, nodeId, false, false);

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
      IOLabAuthorization auth,
      uint mapId)
    {
      Logger.LogDebug($"GetMapNodesAsync(uint mapId={mapId})");

      // test if user has access to map.
      if (!auth.HasAccess("R", Utils.Constants.ScopeLevelMap, mapId))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

      var map = await MapsReaderWriter.Instance(Logger.GetLogger(), dbContext).GetSingleAsync(mapId);
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
      IOLabAuthorization auth,
      uint mapId,
      uint nodeId,
      PostNewLinkRequest body)
    {
      Logger.LogDebug($"PostMapNodeLinkAsync( destinationId = {body.DestinationId})");

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
        dbContext.Entry(phys).State = EntityState.Added;

        await dbContext.SaveChangesAsync();

        var dto = new PostNewLinkResponse
        {
          Id = phys.Id
        };

        return dto;
      }
      catch (Exception ex)
      {
        Logger.LogError(ex, "PostMapNodeLinkAsync");
        throw;
      }
    }

    /// <summary>
    /// Create a new node
    /// </summary>
    /// <returns>IActionResult</returns>
    public async Task<PostNewNodeResponse> PostMapNodesAsync(
      IOLabAuthorization auth,
      PostNewNodeRequest body)
    {
      Logger.LogDebug($"PostMapNodesAsync(x = {body.X}, y = {body.Y}, sourceId = {body.SourceId})");

      using var transaction = dbContext.Database.BeginTransaction();

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
        dbContext.Entry(phys).State = EntityState.Added;

        await dbContext.SaveChangesAsync();

        var link = MapNodeLinks.CreateDefault();
        link.MapId = sourceNode.MapId;
        link.NodeId1 = body.SourceId;
        link.NodeId2 = phys.Id;
        dbContext.Entry(link).State = EntityState.Added;

        await dbContext.SaveChangesAsync();
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
    /// Update a given map's nodegrid
    /// </summary>
    /// <param name="mapId">map id</param>
    /// <param name="body">nodegrid DTO</param>
    /// <returns>IActionResult</returns>
    public async Task<bool> PutMapNodegridAsync(
      IOLabAuthorization auth,
      uint mapId,
      PutNodeGridRequest[] body)
    {
      if (0 == body.Length)
        return true;

      using var transaction = dbContext.Database.BeginTransaction();

      try
      {
        var map = GetSimple(dbContext, mapId);

        if (map == null)
          throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapId);

        // test if user has access to map.
        if (!auth.HasAccess("W", Utils.Constants.ScopeLevelMap, map.Id))
          throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, map.Id);

        foreach (var nodeDto in body)
        {
          var phys = dbContext.MapNodes.FirstOrDefault(x => x.Id == nodeDto.Id);

          if (null == phys)
            throw new Exception("Bad request"); // replace with oLabBadRequestException once implemented

          if (phys.MapId != map.Id)
            throw new Exception("Bad request"); // replace with oLabBadRequestException once implemented

          phys.Text = nodeDto.Text;
          phys.Title = nodeDto.Title;
          phys.X = nodeDto.X;
          phys.Y = nodeDto.Y;

          dbContext.MapNodes.Update(phys);
        }

        await dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return true;
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
    /// <param name="context"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public Model.MapNodeLinks GetLinkSimple(OLabDBContext context, uint id)
    {
      var phys = context.MapNodeLinks.FirstOrDefault(x => x.Id == id);
      return phys;
    }

    /// <summary>
    /// Delete a node link
    /// </summary>
    /// <returns>IActionResult</returns>
    public async Task<bool> DeleteMapNodeLinkAsync(
      IOLabAuthorization auth,
      uint mapId,
      uint linkId)
    {
      Logger.LogDebug($"DeleteMapNodeLinkAsync(mapId = {mapId}, linkId = {linkId})");

      try
      {
        // test if user has access to map.
        if (!auth.HasAccess("W", Utils.Constants.ScopeLevelMap, mapId))
          throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

        var link = GetLinkSimple(dbContext, linkId);

        if (link == null)
          throw new OLabObjectNotFoundException("Links", linkId);

        Logger.LogDebug($"deleting link {link.Id} of map {link.MapId}");
        dbContext.MapNodeLinks.Remove(link);
        await dbContext.SaveChangesAsync();
        return true;
      }
      catch (Exception ex)
      {
        Logger.LogError(ex, "DeleteMapNodeLinkAsync");
        throw;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Dto.Designer.ScopedObjectsDto> GetScopedObjectsRawAsync(
      IOLabAuthorization auth,
      uint id)
    {
      Logger.LogDebug($"MapsController.GetScopedObjectsRawAsync(uint id={id})");

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
    public async Task<Dto.Designer.ScopedObjectsDto> GetScopedObjectsAsync(
      IOLabAuthorization auth,
      uint id)
    {
      Logger.LogDebug($"MapsController.GetScopedObjectsTranslatedAsync(uint id={id})");

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
    public async Task<Dto.Designer.ScopedObjectsDto> GetScopedObjectsAsync(
      uint id,
      bool enableWikiTranslation)
    {
      var map = GetSimple(dbContext, id);
      if (map == null)
        return null;

      var phys = await GetScopedObjectsAllAsync(
        map.Id, 
        Utils.Constants.ScopeLevelMap, 
        _fileStorageModule);

      var physServer = await GetScopedObjectsAllAsync(
        1, 
        Utils.Constants.ScopeLevelServer, 
        _fileStorageModule);

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

      var builder = new ObjectMapper.Designer.ScopedObjects(Logger, _wikiTagProvider, enableWikiTranslation);
      var dto = builder.PhysicalToDto(phys);

      var maps = dbContext.Maps.Select(x => new IdName() { Id = x.Id, Name = x.Name }).ToList();
      var nodes = dbContext.MapNodes.Select(x => new IdName() { Id = x.Id, Name = x.Title }).ToList();
      var servers = dbContext.Servers.Select(x => new IdName() { Id = x.Id, Name = x.Name }).ToList();

      foreach (var question in dto.Questions)
        question.ParentInfo = FindParentInfo(question.ScopeLevel, question.ParentId, maps, nodes, servers);

      foreach (var constant in dto.Constants)
        constant.ParentInfo = FindParentInfo(constant.ScopeLevel, constant.ParentId, maps, nodes, servers);

      foreach (var counter in dto.Counters)
        counter.ParentInfo = FindParentInfo(counter.ScopeLevel, counter.ParentId, maps, nodes, servers);

      foreach (var file in dto.Files)
        file.ParentInfo = FindParentInfo(file.ScopeLevel, file.ParentId, maps, nodes, servers);

      foreach (var script in dto.Scripts)
        script.ParentInfo = FindParentInfo(script.ScopeLevel, script.ParentId, maps, nodes, servers);

      return dto;
    }

    /// <summary>
    /// Get a list of security users attached to a map
    /// </summary>
    /// <param name="map">Relevent map object</param>
    /// <returns></returns>
    public IList<Users> GetMapAccessCandidates(Maps map, String search)
    {
      if (map == null)
        return null;

      search = search.Trim();

      var users = dbContext.Users.Where(x => search.Length > 0 ? (
        x.Nickname.Contains(search)
        || x.Email.Contains(search)
        || x.Username.Contains(search)
      ) : true).Take(20).ToList();

      return users;
    }

    /// <summary>
    /// Insert a user to the users table
    /// </summary>
    /// <param name="map">Relevent map object</param>
    /// <returns></returns>
    public async Task<int> PutMapAccessCandidateAsync(
      Maps map,
      MapAccessCandidateRequest body
    )
    {
      if (map == null)
        return 0;

      if (String.IsNullOrEmpty(body.Email?.Trim()))
        throw new OLabBadRequestException("User email cannot be empty.");

      if (String.IsNullOrEmpty(body.Username?.Trim()))
        throw new OLabBadRequestException("Username cannot be empty.");

      if (!GenericValidations.IsValidEmail(body.Email))
        throw new OLabBadRequestException("Invalid email address.");

      if (!GenericValidations.IsValidUsername(body.Username))
        throw new OLabBadRequestException("Invalid username (can only contain alphanumeric and any of -_ characters).");

      var existing = dbContext.Users.Where(u =>
        u.Username.ToLower() == body.Username.ToLower()
        || u.Email.ToLower() == body.Email.ToLower()).FirstOrDefault();

      // email and/or username taken
      if (null != existing)
        throw new OLabBadRequestException("Username and/or email already taken.");

      var phys = Users.CreateDefault(new AddUserRequest
      {
        Username = body.Username,
        NickName = body.Username,
        EMail = body.Email,
      });

      await dbContext.Users.AddAsync(phys);
      var id = await dbContext.SaveChangesAsync();

      return (int)phys.Id;
    }

    /// <summary>
    /// Get a list of security users attached to a map
    /// </summary>
    /// <param name="map">Relevent map object</param>
    /// <returns></returns>
    public IList<SecurityUsers> GetSecurityUsersRaw(Maps map)
    {
      if (map == null)
        return null;

      var users = dbContext.SecurityUsers.Where(x => x.ImageableId == map.Id
      && (
        // note: this excludes `ImageableType == "*"` entries, allowing authors
        // to manipulate those rows may lead to unwanted side-effects
        x.ImageableType == Utils.Constants.ScopeLevelMap /*|| x.ImageableType == "*"*/
      )).ToList();

      return users;
    }

    /// <summary>
    /// Assign security user to map (inserts and patches)
    /// </summary>
    /// <param name="map">Relevent map object</param>
    /// <returns></returns>
    public async Task<bool> SetMapSecurityUserAsync(
      Maps map,
      AssignSecurityUserRequest body
    )
    {
      if (map == null)
        return false;

      body.CheckAcl();

      var user = dbContext.Users.Where(u => u.Id == body.UserId).FirstOrDefault();

      if (null == user)
        throw new OLabBadRequestException("User not found.");

      var securityUser = dbContext.SecurityUsers.SingleOrDefault(x => x.ImageableId == map.Id
      && (
        // note: this excludes `ImageableType == "*"` entries, allowing authors
        // to manipulate those rows may lead to unwanted side-effects
        x.ImageableType == Utils.Constants.ScopeLevelMap
        && x.UserId == body.UserId
      ));

      if (null == securityUser)
        securityUser = new SecurityUsers();

      securityUser.ImageableId = map.Id;
      securityUser.UserId = user.Id;
      securityUser.ImageableType = Utils.Constants.ScopeLevelMap;
      securityUser.Acl = body.Acl;

      try
      {
        if (securityUser.Id > 0) // update existing security user
        {
          dbContext.SecurityUsers.Update(securityUser);
          var id = await dbContext.SaveChangesAsync();
          return id > 0;
        }
        else // insert security user
        {
          await dbContext.SecurityUsers.AddAsync(securityUser);
          var id = await dbContext.SaveChangesAsync();
          return id > 0;
        }
      }
      catch (Exception)
      {
        return false;
      }
    }

    /// <summary>
    /// Unassign security user from map (delete)
    /// </summary>
    /// <param name="map">Relevent map object</param>
    /// <param name="userId">Relevent user id</param>
    /// <returns></returns>
    public async Task<bool> UnsetMapSecurityUserAsync(
      Maps map,
      uint userId
    )
    {
      if (map == null)
        return false;

      var securityUser = dbContext.SecurityUsers.SingleOrDefault(x => x.ImageableId == map.Id
      && (
        // note: this excludes `ImageableType == "*"` entries, allowing authors
        // to manipulate those rows may lead to unwanted side-effects
        x.ImageableType == Utils.Constants.ScopeLevelMap
        && x.UserId == userId
      ));

      if (null == securityUser)
        return true;

      try
      {
        dbContext.SecurityUsers.Remove(securityUser);
        var changes = await dbContext.SaveChangesAsync();
        return changes > 0;
      }
      catch (Exception)
      {
        return false;
      }
    }
  }
}