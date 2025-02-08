using Microsoft.EntityFrameworkCore;
using OLab.Access.Interfaces;
using OLab.Api.Data.Exceptions;
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
    OLabDBContext dbContext) : base( logger, configuration, dbContext )
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
    GetLogger().LogInformation( $"{auth.OLabUser.Id}: SessionEndpoint.ReadAsync" );

    var session = await GetDbContext().UserSessions
      .Include( session => session.Statements )
      .Include( session => session.UserBookmarks )
      .Include( session => session.UserSessiontraces )
      .FirstOrDefaultAsync( x => x.Uuid == sessionUuid );

    if ( session == null )
      throw new OLabObjectNotFoundException( "UserSession", sessionUuid );

    var dto = new SessionMapper(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider() ).PhysicalToDto( session );

    return dto;
  }
}
