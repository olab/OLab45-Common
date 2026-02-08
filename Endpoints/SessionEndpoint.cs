using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using OLab.Access.Interfaces;
using OLab.Api.Data.Exceptions;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Contracts;
using OLab.Common.Interfaces;
using OLab.Data.Dtos.Session;
using OLab.Data.Mappers;
using System;
using System.Linq;
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

  public async Task<SessionStatistics> GetSessionStats(string sessionUuid)
  {
    var sessionStats = new SessionStatistics();

    try
    {
      sessionStats.SessionId = sessionUuid;

      var session = await GetDbContext().UserSessions
        .Include( session => session.UserSessiontraces )
        .FirstOrDefaultAsync( x => x.Uuid == sessionUuid );

      var firstSessionEntry = session.UserSessiontraces.Min( x => x.DateStamp );
      if ( firstSessionEntry.HasValue )
        sessionStats.SessionStart = Conversions.GetTime( firstSessionEntry.Value );
      else
        sessionStats.SessionStart = DateTime.UtcNow;

      TimeSpan timeSpan = DateTime.UtcNow - sessionStats.SessionStart.Value;
      sessionStats.SessionDuration = timeSpan;

      sessionStats.NodeCount = session.UserSessiontraces.Count();

    }
    catch ( Exception ex)
    {
      GetLogger().LogError( ex, "Error in GetSessionStats" );
    }

    return sessionStats;

  }


}
