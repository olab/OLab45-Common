using Microsoft.Extensions.Logging;
using OLabWebAPI.Data.Interface;
using OLabWebAPI.Model;
using System;
using System.Linq;

namespace OLabWebAPI.Data.Session
{
  public class OLabSession : IOLabSession
  {
    // private readonly AppSettings _appSettings;
    private readonly OLabDBContext _context;
    private readonly IUserContext _userContext;
    private readonly ILogger _logger;
    private string _sessionId;

    public OLabSession(ILogger logger, OLabDBContext context, IUserContext userContext)
    {
      // _appSettings = appSettings.Value;
      _context = context;
      _userContext = userContext;
      _logger = logger;
    }

    public void SetSessionId(string sessionId)
    {
      _sessionId = sessionId;
    }

    public string GetSessionId()
    {
      return _sessionId;
    }

    public void OnStartSession(string userName, uint mapId, string ipAddress)
    {
      _sessionId = IOLabSession.GenerateSessionId();

      UserSessions session = new UserSessions
      {
        Uuid = _sessionId,
        MapId = mapId,
        StartTime = GetUnixTime(),
        UserIp = ipAddress
      };

      _context.UserSessions.Add(session);
      _context.SaveChanges();

      _logger.LogInformation($"OnStartSession: session {_sessionId} ({userName}) MapId: {mapId}. Session PK: {session.Id}");
    }

    public void OnEndSession(uint mapId, uint nodeId)
    {
      _logger.LogInformation($"OnEndSession: session {GetSessionId()} Map: {mapId} Node: {nodeId}");

      UserSessions session = GetSession(GetSessionId());
      if (session == null)
        return;

      session.EndTime = GetUnixTime();

      _context.UserSessions.Update(session);
      _context.SaveChanges();
    }

    public void OnPlayNode(uint mapId, uint nodeId)
    {
      _logger.LogInformation($"OnPlayNode: session {GetSessionId()} Map: {mapId} Node: {nodeId}");

      UserSessions session = GetSession(GetSessionId());
      if (session == null)
        return;

      UserSessionTraces sessionTrace = new UserSessionTraces
      {
        SessionId = session.Id,
        MapId = mapId,
        NodeId = nodeId,
        DateStamp = GetUnixTime()
      };

      _context.UserSessionTraces.Add(sessionTrace);
      _context.SaveChanges();

    }

    public void OnQuestionResponse(uint mapId, uint nodeId, uint questionId, string value)
    {
      _logger.LogInformation($"OnQuestionResponse: session {GetSessionId()} Map: {mapId} Node: {nodeId} Question: {questionId} = {value} ");

      UserSessions session = GetSession(GetSessionId());
      if (session == null)
        return;

      UserResponses userResponse = new UserResponses
      {
        SessionId = session.Id,
        QuestionId = questionId,
        Response = value,
        NodeId = nodeId,
        CreatedAt = GetUnixTime()
      };

      _context.UserResponses.Add(userResponse);
      _context.SaveChanges();
    }

    private decimal GetUnixTime()
    {
      TimeSpan span = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
      double unixTime = span.TotalSeconds;
      return (decimal)unixTime;
    }

    private UserSessions GetSession(string sessionId)
    {
      UserSessions session = _context.UserSessions.Where(x => x.Uuid == sessionId).FirstOrDefault();
      if (session == null)
      {
        _logger.LogError($"Unable to get session, sessionId '{sessionId}' not found");
        return null;
      }

      return session;
    }
  }
}