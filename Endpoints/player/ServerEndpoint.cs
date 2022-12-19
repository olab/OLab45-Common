using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLabWebAPI.Model;
using OLabWebAPI.Dto;
using OLabWebAPI.ObjectMapper;
using OLabWebAPI.Common;
using OLabWebAPI.Data.Interface;
using OLabWebAPI.Utils;
using OLabWebAPI.Dto.Designer;

namespace OLabWebAPI.Endpoints.Player
{
  public partial class ServerEndpoint : OlabEndpoint
  {

    public ServerEndpoint(
      OLabLogger logger,
      OLabDBContext context) : base(logger, context)
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

      logger.LogDebug(string.Format("found {0} servers", items.Count));

      return new OLabAPIPagedResponse<Servers> { Data = items, Remaining = remaining, Count = total };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serverId"></param>
    /// <returns></returns>
    public async Task<OLabWebAPI.Dto.ScopedObjectsDto> GetScopedObjectsRawAsync(uint serverId)
    {
      logger.LogDebug($"ServerController.GetScopedObjectsRawAsync(uint serverId={serverId})");
      var dto = await GetScopedObjectsAsync(serverId, false);
      return dto;      
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serverId"></param>
    /// <returns></returns>
    public async Task<OLabWebAPI.Dto.ScopedObjectsDto> GetScopedObjectsTranslatedAsync(uint serverId)
    {
      logger.LogDebug($"ServerController.GetScopedObjectsTranslatedAsync(uint serverId={serverId})");
      var dto = await GetScopedObjectsAsync(serverId, true);
      return dto;      
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serverId"></param>
    /// <param name="enableWikiTranslation"></param>
    /// <returns></returns>
    public async Task<OLabWebAPI.Dto.ScopedObjectsDto> GetScopedObjectsAsync(
      uint serverId,
      bool enableWikiTranslation)
    {
      logger.LogDebug($"ServerController.GetScopedObjectsAsync(uint serverId={serverId})");

      var phys = await GetScopedObjectsAllAsync(serverId, Utils.Constants.ScopeLevelServer);
      var builder = new ObjectMapper.ScopedObjects(logger, enableWikiTranslation);
      var dto = builder.PhysicalToDto(phys);

      return dto;
    }
  }
}
