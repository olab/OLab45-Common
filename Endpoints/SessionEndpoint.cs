using Microsoft.EntityFrameworkCore;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using OLab.Data.Dtos.Session;
using OLab.Data.Mappers;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints;

public partial class SessionEndpoint : OLabEndpoint
{
  public SessionEndpoint(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext dbContext) : base(logger, configuration, dbContext)
  {
  }

  /// <summary>
  /// Retrieve a map
  /// </summary>
  /// <param name="id">Map Id</param>
  /// <returns>Map</returns>
  public async Task<SessionDto> GetAsync(
    IOLabAuthorization auth,
    string sessionUuid)
  {
    Logger.LogDebug($"{auth.UserContext.UserId}: SessionEndpoint.GetAsync");

    var session = await dbContext.UserSessions
      .Include(session => session.Statements)
      .Include(session => session.UserBookmarks)
      .Include(session => session.UserSessionTraces)
      .FirstOrDefaultAsync(x => x.Uuid == sessionUuid);

    if (session == null)
      throw new OLabObjectNotFoundException("UserSession", sessionUuid);

    var dto = new SessionMapper(Logger).PhysicalToDto(session);

    return dto;
  }
}
