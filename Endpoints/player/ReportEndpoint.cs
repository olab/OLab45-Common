using Data.Contracts;
using Microsoft.EntityFrameworkCore;
using OLabWebAPI.Common.Exceptions;
using OLabWebAPI.Dto;
using OLabWebAPI.Model;
using OLabWebAPI.Utils;
using System.Linq;
using System.Threading.Tasks;

namespace OLabWebAPI.Endpoints.Player
{
  public partial class ReportEndpoint : OlabEndpoint
  {
    public ReportEndpoint(
      OLabLogger logger,
      OLabDBContext context) : base(logger, context)
    {
    }

    public async Task<SessionReport> GetAsync(string contextId)
    {
      var dto = new SessionReport();

      UserSessions session = await dbContext.UserSessions.FirstOrDefaultAsync(x => x.Uuid == contextId);
      if (session == null)
        throw new OLabObjectNotFoundException("Session", contextId);

      var sessionTraces = dbContext.UserSessionTraces
        .Where(x => x.SessionId == session.Id)
        .OrderBy(x => x.DateStamp).ToList();
      logger.LogDebug($"Found {sessionTraces.Count} session trace records for session {contextId}");

      var sessionResponses = dbContext.UserResponses
        .Where(x => x.SessionId == session.Id)
        .OrderBy(x => x.CreatedAt).ToList();
      logger.LogDebug($"Found {sessionResponses.Count} session response records for session {contextId}");

      var sessionState = dbContext.UserState
        .Where(x => x.SessionId == session.Id);
      logger.LogDebug($"Found {sessionResponses.Count} session state records for session {contextId}");


      return dto;
    }
  }
}
