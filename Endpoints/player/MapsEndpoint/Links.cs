using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLabWebAPI.Common;
using OLabWebAPI.Common.Exceptions;
using OLabWebAPI.Dto;
using OLabWebAPI.Interface;
using OLabWebAPI.Model;
using OLabWebAPI.Model.ReaderWriter;
using OLabWebAPI.ObjectMapper;
using OLabWebAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
      IOlabAuthentication auth, 
      uint mapId, 
      uint nodeId, 
      uint linkId, 
      MapNodeLinksFullDto linkdto)
    {
      logger.LogDebug($"PutMapNodeLinksAsync(uint mapId={mapId}, nodeId={nodeId}, linkId={linkId})");

      // test if user has access to map.
      if (!auth.HasAccess("W", Utils.Constants.ScopeLevelMap, mapId))
        throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

      try
      {
        var builder = new MapNodeLinksFullMapper(logger);
        var phys = builder.DtoToPhysical(linkdto);

        context.Entry(phys).State = EntityState.Modified;
        await context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        var existingMap = GetLinkSimple(context, linkId);
        if (existingMap == null)
          throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapId);
      }

    }

  }
  
}
