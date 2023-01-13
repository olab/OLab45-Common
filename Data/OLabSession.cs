using Microsoft.Extensions.Logging;
using OLabWebAPI.Data.Interface;
using OLabWebAPI.Model;
using System;
using System.Linq;
using OLabWebAPI.Utils;

namespace OLabWebAPI.Data
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

    public void OnStartSession(IUserContext userContext, uint mapId)
    {
      _sessionId = IOLabSession.GenerateSessionId();

      _logger.LogInformation($"generated a new contextId: {_sessionId}");

      UserSessions session = new UserSessions
      {
        Uuid = _sessionId,
        MapId = mapId,
        StartTime = Conversions.GetCurrentUnixTime(),
        UserIp = userContext.IPAddress,
        Iss = userContext.Issuer,
        UserId = userContext.UserId
      };

      _context.UserSessions.Add(session);
      _context.SaveChanges();

      _logger.LogInformation($"OnStartSession: session {_sessionId} ({userContext.UserName}) MapId: {mapId}. Session PK: {session.Id}");
    }

    public void OnEndSession(uint mapId, uint nodeId)
    {
      _logger.LogInformation($"OnEndSession: session {GetSessionId()} Map: {mapId} Node: {nodeId}");

      UserSessions session = GetSession(GetSessionId());
      if (session == null)
        return;

      session.EndTime = Conversions.GetCurrentUnixTime();

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
        DateStamp = Conversions.GetCurrentUnixTime()
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
        CreatedAt = Conversions.GetCurrentUnixTime()
      };

      _context.UserResponses.Add(userResponse);
      _context.SaveChanges();
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