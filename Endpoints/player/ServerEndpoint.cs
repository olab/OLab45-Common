using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;
using OLab.Data;
using OLab.Data.Interface;
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
  public async Task<Dto.ScopedObjectsDto> GetScopedObjectsTranslatedAsync(uint serverId)
  {
    GetLogger().LogInformation( $"ServerEndpoint.GetScopedObjectsTranslatedAsync(uint serverId={serverId})" );
    var dto = await GetScopedObjectsAsync( serverId, true );
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

    await phys.AddScopeFromDatabaseAsync( Utils.Constants.ScopeLevelServer, 1 );

    var builder = new ScopedObjectsMapper(
      GetLogger(),
      GetDbContext(),
      GetWikiProvider(),
      enableWikiTranslation );

    var dto = builder.PhysicalToDto( phys );

    return dto;
  }
}
