using Microsoft.EntityFrameworkCore;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints.Player;

public partial class MapsEndpoint : OLabEndpoint
{
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
  /// Saves a link edit
  /// </summary>
  /// <param name="mapId">map id</param>
  /// <param name="nodeId">node id</param>
  /// <param name="linkId">link id</param>
  /// <returns>IActionResult</returns>
  public async Task PutMapNodeLinksAsync(
    IOLabAuthorization auth,
    uint mapId,
    uint nodeId,
    uint linkId,
    MapNodeLinksFullDto linkdto)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: MapsEndpoint.PutMapNodeLinksAsync");

    // test if user has access to map.
    if (!auth.HasAccess("W", Utils.Constants.ScopeLevelMap, mapId))
      throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

    try
    {
      var builder = new MapNodeLinksFullMapper(Logger);
      var phys = builder.DtoToPhysical(linkdto);

      dbContext.Entry(phys).State = EntityState.Modified;
      await dbContext.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
      var existingMap = GetLinkSimple(dbContext, linkId);
      if (existingMap == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapId);
    }

  }

}
