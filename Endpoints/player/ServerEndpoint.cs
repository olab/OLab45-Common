using Microsoft.IdentityModel.Protocols;
using OLab.Access.Interfaces;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Contracts;
using OLab.Common.Interfaces;
using OLab.Data;
using OLab.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints.Player;

public partial class ServerEndpoint : OLabEndpoint
{
  private SessionEndpoint _sessionEndpoint;

  public ServerEndpoint(
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
        fileStorageProvider )
  {
    _sessionEndpoint = new SessionEndpoint( logger, configuration, context );
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="serverId"></param>
  /// <returns></returns>
  public async Task<Dto.ScopedObjectsDto> GetScopedObjectsRawAsync(uint serverId)
  {
    GetLogger().LogInformation( $"ServerEndpoint.GetScopedObjectsRawAsync(uint serverId={serverId})" );
    var dto = await GetScopedObjectsAsync( serverId, false );
    return dto;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="serverId"></param>
  /// <returns></returns>
  public async Task<Dto.ScopedObjectsDto> GetScopedObjectsTranslatedAsync(
    uint serverId,
    IOLabAuthorization auth,
    Dictionary<string, IEnumerable<string>> headers)
  {
    GetLogger().LogInformation( $"ServerEndpoint.GetScopedObjectsTranslatedAsync(uint serverId={serverId})" );
    var dto = await GetScopedObjectsAsync( serverId, true );

    var sessionId = headers.TryGetValue( "OlabSessionId", out var sessionIds ) ?
      sessionIds.FirstOrDefault() ??
      string.Empty : string.Empty;

    dto.Constants.Add(
      new Dto.ConstantsDto
      {
        Id = 0,
        Name = "LoginId",
        Value = auth.OLabUser.Username,
        ImageableId = 1,
        ImageableType = "Server",
        IsSystem = 1,
        CreatedAt = DateTime.UtcNow
      } );

    dto.Constants.Add(
      new Dto.ConstantsDto
      {
        Id = 0,
        Name = "UserName",
        Value = auth.OLabUser.Nickname,
        ImageableId = 1,
        ImageableType = "Server",
        IsSystem = 1,
        CreatedAt = DateTime.UtcNow
      } );

    dto.Constants.Add(
      new Dto.ConstantsDto
      {
        Id = 0,
        Name = "UserId",
        Value = auth.OLabUser.Id.ToString(),
        ImageableId = 1,
        ImageableType = "Server",
        IsSystem = 1,
        CreatedAt = DateTime.UtcNow
      } );


    if ( !string.IsNullOrEmpty( sessionId ) )
    {
      SessionStatistics sessionStats = await _sessionEndpoint.GetSessionStats( sessionId );

      dto.Constants.Add(
        new Dto.ConstantsDto
        {
          Id = 0,
          Name = "SessionId",
          Value = sessionStats.SessionId,
          ImageableId = 1,
          ImageableType = "Server",
          IsSystem = 1,
          CreatedAt = DateTime.UtcNow
        } );

      dto.Constants.Add(
        new Dto.ConstantsDto
        {
          Id = 0,
          Name = "SessionTimeStamp",
          Value = sessionStats.SessionStart.HasValue ? $"{sessionStats.SessionStart.Value.ToString()} UTC" : "<unknown>",
          ImageableId = 1,
          ImageableType = "Server",
          IsSystem = 1,
          CreatedAt = DateTime.UtcNow
        } );

      dto.Constants.Add(
        new Dto.ConstantsDto
        {
          Id = 0,
          Name = "SessionDuration",
          Value = Math.Floor( sessionStats.SessionDuration.TotalSeconds ).ToString(),
          ImageableId = 1,
          ImageableType = "Server",
          IsSystem = 1,
          CreatedAt = DateTime.UtcNow
        } );

      dto.Constants.Add(
        new Dto.ConstantsDto
        {
          Id = 0,
          Name = "NodesVisited",
          Value = sessionStats.NodeCount.ToString(),
          ImageableId = 1,
          ImageableType = "Server",
          IsSystem = 1,
          CreatedAt = DateTime.UtcNow
        } );
    }

    return dto;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="serverId"></param>
  /// <param name="enableWikiTranslation"></param>
  /// <returns></returns>
  public async Task<Dto.ScopedObjectsDto> GetScopedObjectsAsync(
    uint serverId,
    bool enableWikiTranslation)
  {
    GetLogger().LogInformation( $"ServerEndpoint.GetScopedObjectsAsync(uint serverId={serverId})" );

    var phys = new ScopedObjects(
      GetLogger(),
      GetDbContext(),
      GetWikiProvider(), _fileStorageModule );

    await phys.LoadScopedObjectsFromDatabaseAsync( Utils.Constants.ScopeLevelServer, 1 );

    var builder = new ScopedObjectsMapper(
      GetLogger(),
      GetDbContext(),
      GetWikiProvider(),
      enableWikiTranslation );

    var dto = builder.PhysicalToDto( phys );

    return dto;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="serverId"></param>
  /// <returns></returns>
  public async Task<Dto.ScopedObjectsDto> GetDynamicObjectsRawAsync(uint serverId)
  {
    GetLogger().LogInformation( $"ServerEndpoint.GetDynamicObjectsRawAsync(uint serverId={serverId})" );
    var dto = await GetDynamicObjectsAsync( serverId, false );
    return dto;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="serverId"></param>
  /// <returns></returns>
  public async Task<Dto.ScopedObjectsDto> GetDynamicObjectsTranslatedAsync(uint serverId)
  {
    GetLogger().LogInformation( $"ServerEndpoint.GetDynamicObjectsTranslatedAsync(uint serverId={serverId})" );
    var dto = await GetDynamicObjectsAsync( serverId, true );
    return dto;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="serverId"></param>
  /// <param name="enableWikiTranslation"></param>
  /// <returns></returns>
  public async Task<Dto.ScopedObjectsDto> GetDynamicObjectsAsync(
    uint serverId,
    bool enableWikiTranslation)
  {
    GetLogger().LogInformation( $"ServerEndpoint.GetDynamicObjectsAsync(uint serverId={serverId})" );

    var phys = new ScopedObjects(
      GetLogger(),
      GetDbContext(),
      GetWikiProvider(), _fileStorageModule );

    await phys.LoadDynamicObjectsFromDatabaseAsync( Utils.Constants.ScopeLevelServer, 1 );

    var builder = new ScopedObjectsMapper(
      GetLogger(),
      GetDbContext(),
      GetWikiProvider(),
      enableWikiTranslation );

    var dto = builder.PhysicalToDto( phys );

    return dto;
  }

}
