using Microsoft.EntityFrameworkCore;
using OLab.Common.Exceptions;
using OLab.Data.Exceptions;
using OLab.Data.Interface;
using OLab.Dto;
using OLab.Model;
using OLab.ObjectMapper;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Endpoints.Player
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
