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
