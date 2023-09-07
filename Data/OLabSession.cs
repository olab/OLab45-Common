using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Utils;
using System.Linq;
using System.Text;

namespace OLab.Api.Data
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

      var session = new UserSessions
      {
        Uuid = _sessionId,
        MapId = mapId,
        StartTime = Conversions.GetCurrentUnixTime(),
        UserIp = userContext.IPAddress,
        Iss = userContext.Issuer,
        UserId = userContext.UserId,
        CourseName = userContext.ReferringCourse
      };

      _context.UserSessions.Add(session);
      _context.SaveChanges();

      _logger.LogInformation($"OnStartSession: session {_sessionId} ({userContext.UserName}) MapId: {mapId}. Session PK: {session.Id}");
    }

    public void OnExtendSession(uint mapId, uint nodeId)
    {
      _logger.LogInformation($"OnExtendSession: session {GetSessionId()} Map: {mapId} Node: {nodeId}");

      var session = GetSession(GetSessionId());
      if (session == null)
        return;

      session.EndTime = Conversions.GetCurrentUnixTime();

      _context.UserSessions.Update(session);
      _context.SaveChanges();
    }

    public void OnPlayNode(uint mapId, uint nodeId)
    {
      _logger.LogInformation($"OnPlayNode: session {GetSessionId()} Map: {mapId} Node: {nodeId}");

      var session = GetSession(GetSessionId());
      if (session == null)
        return;

      var sessionTrace = new UserSessionTraces
      {
        SessionId = session.Id,
        MapId = mapId,
        NodeId = nodeId,
        DateStamp = Conversions.GetCurrentUnixTime(),
        UserId = session.UserId
      };

      _context.UserSessionTraces.Add(sessionTrace);
      _context.SaveChanges();

    }

    public void OnQuestionResponse(uint mapId, uint nodeId, uint questionId, string value)
    {
      _logger.LogInformation($"OnQuestionResponse: session {GetSessionId()} Map: {mapId} Node: {nodeId} Question: {questionId} = {value} ");

      var session = GetSession(GetSessionId());
      if (session == null)
        return;

      // truncate the message in case it's too long
      if (string.IsNullOrEmpty(value) && (value.Length > 1000))
        value = value[997..] + "...";

      var userResponse = new UserResponses
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

    public void SaveSessionState(uint mapId, uint nodeId, DynamicScopedObjectsDto dynamicObjects)
    {
      var userState = new UserState
      {
        MapId = mapId,
        MapNodeId = nodeId,
        UserId = _userContext.UserId,
        StateData = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(dynamicObjects))
      };

      _context.UserState.Add(userState);
      _context.SaveChanges();
    }

    private UserSessions GetSession(string sessionId)
    {
      var session = _context.UserSessions.Where(x => x.Uuid == sessionId).FirstOrDefault();
      if (session == null)
      {
        _logger.LogError($"Unable to get session, sessionId '{sessionId}' not found");
        return null;
      }

      return session;
    }
  }
}