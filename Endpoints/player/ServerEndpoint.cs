using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints.Player
{
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
          fileStorageProvider)
    {
    }

    /// <summary>
    /// Get a list of servers
    /// </summary>
    /// <param name="take">Max number of records to return</param>
    /// <param name="skip">SKip over a number of records</param>
    /// <returns>IActionResult</returns>
    public async Task<OLabAPIPagedResponse<Servers>> GetAsync([FromQuery] int? take, [FromQuery] int? skip)
    {
      var items = new List<Servers>();
      var total = 0;
      var remaining = 0;

      if (!skip.HasValue)
        skip = 0;

      if (take.HasValue && skip.HasValue)
      {
        items = await dbContext.Servers.Skip(skip.Value).Take(take.Value).OrderBy(x => x.Name).ToListAsync();
        remaining = total - take.Value - skip.Value;
      }
      else
      {
        items = await dbContext.Servers.OrderBy(x => x.Name).ToListAsync();
      }

      total = items.Count;

      Logger.LogDebug(string.Format("found {0} servers", items.Count));

      return new OLabAPIPagedResponse<Servers> { Data = items, Remaining = remaining, Count = total };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serverId"></param>
    /// <returns></returns>
    public async Task<Dto.ScopedObjectsDto> GetScopedObjectsRawAsync(uint serverId)
    {
      Logger.LogDebug($"ServerEndpoint.GetScopedObjectsRawAsync(uint serverId={serverId})");
      var dto = await GetScopedObjectsAsync(serverId, false);
      return dto;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serverId"></param>
    /// <returns></returns>
    public async Task<Dto.ScopedObjectsDto> GetScopedObjectsTranslatedAsync(uint serverId)
    {
      Logger.LogDebug($"ServerEndpoint.GetScopedObjectsTranslatedAsync(uint serverId={serverId})");
      var dto = await GetScopedObjectsAsync(serverId, true);
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
      Logger.LogDebug($"ServerEndpoint.GetScopedObjectsAsync(uint serverId={serverId})");

      var phys = await GetScopedObjectsAllAsync(serverId, Utils.Constants.ScopeLevelServer, _fileStorageModule);
      var builder = new ObjectMapper.ScopedObjects(Logger, _wikiTagProvider, enableWikiTranslation);
      var dto = builder.PhysicalToDto(phys);

      return dto;
    }
  }
}
