using Microsoft.EntityFrameworkCore;
using OLabWebAPI.Common.Exceptions;
using OLabWebAPI.Data.Exceptions;
using OLabWebAPI.Data.Interface;
using OLabWebAPI.Dto;
using OLabWebAPI.Model;
using OLabWebAPI.ObjectMapper;
using System.Linq;
using System.Threading.Tasks;

namespace OLabWebAPI.Endpoints.Player
{
  public partial class MapsEndpoint : OlabEndpoint
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public Model.MapNodeLinks GetLinkSimple(OLabDBContext context, uint id)
    {
      MapNodeLinks phys = context.MapNodeLinks.FirstOrDefault(x => x.Id == id);
      return phys;
    }

    /// <summary>
    /// Saves a link edit
    /// </summary>
    /// <param name="mapId">map id</param>
    /// <param name="nodeId">node id</param>
    /// <param name="linkId">link id</param>
    /// <returns>IActionResult</returns>
    public async Task PutMapNodeLinksAsync(
      IOLabAuthentication auth,
      uint mapId,
      uint nodeId,
      uint linkId,
      MapNodeLinksFullDto linkdto)
    {
      logger.LogDebug($"{auth.GetUserContext().UserId}: MapsEndpoint.PutMapNodeLinksAsync");

      // test if user has access to map.
      if (!auth.HasAccess("W", Utils.Constants.ScopeLevelMap, mapId))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

      try
      {
        var builder = new MapNodeLinksFullMapper(logger);
        MapNodeLinks phys = builder.DtoToPhysical(linkdto);

        dbContext.Entry(phys).State = EntityState.Modified;
        await dbContext.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        MapNodeLinks existingMap = GetLinkSimple(dbContext, linkId);
        if (existingMap == null)
          throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapId);
      }

    }

  }

}
