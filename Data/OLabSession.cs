using Dawn;
using DocumentFormat.OpenXml.EMMA;
using Humanizer;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Data.Dtos.ScopedObjects;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace OLab.Api.Data;

public class OLabSession : IOLabSession
{
  private readonly OLabDBContext _dbContext;
  private readonly IUserContext _userContext;
  private readonly IOLabLogger _logger;
  private string _sessionId;
  private uint _mapId;

  public static IOLabSession CreateInstance(
    IOLabLogger logger,
    OLabDBContext context,
    IUserContext userContext)
  {
    return new OLabSession(logger, context, userContext);
  }

  private OLabSession(
    IOLabLogger logger,
    OLabDBContext context,
    IUserContext userContext)
  {
    Guard.Argument(logger, nameof(logger)).NotNull();
    Guard.Argument(context, nameof(context)).NotNull();
    Guard.Argument(userContext, nameof(userContext)).NotNull();

    _dbContext = context;
    _userContext = userContext;
    _logger = logger;

    if (!string.IsNullOrEmpty(_userContext.SessionId))
      SetSessionId(_userContext.SessionId);
  }

  public void SetMapId(uint mapId)
  {
    Guard.Argument(mapId, nameof(mapId)).Positive();
    _mapId = mapId;
  }

  public void SetSessionId(string sessionId)
  {
    Guard.Argument(sessionId, nameof(sessionId)).NotNull();
    _sessionId = sessionId;
  }

  public string GetSessionId()
  {
    return _sessionId;
  }

  /// <summary>
  /// Create a new session on the map
  /// </summary>
  public void OnStartSession()
  {
    Guard.Argument(_mapId, nameof(_mapId)).Positive();

    SetSessionId(IOLabSession.GenerateSessionId());
    _logger.LogInformation($"generated a new session Id: {GetSessionId()}");

    var session = new UserSessions
    {
      Uuid = GetSessionId(),
      MapId = _mapId,
      StartTime = Conversions.GetCurrentUnixTime(),
      UserIp = _userContext.IPAddress,
      Iss = _userContext.Issuer,
      UserId = _userContext.UserId,
      CourseName = _userContext.ReferringCourse
    };

    _dbContext.UserSessions.Add(session);
    _dbContext.SaveChanges();

    _logger.LogInformation($"OnStartSession: session {GetSessionId()} ({_userContext.UserName}) MapId: {_mapId}. Session PK: {session.Id}");
  }

  /// <summary>
  /// Extend the session end time
  /// </summary>
  /// <param name="nodeId">Node Id</param>
  public void OnExtendSessionEnd(uint nodeId)
  {
    Guard.Argument(_mapId, nameof(_mapId)).Positive();

    _logger.LogInformation($"OnExtendSession: session {GetSessionId()} Map: {_mapId} Node: {nodeId}");

    var session = GetSessionFromDatabase(GetSessionId());
    if (session == null)
      return;

    session.EndTime = Conversions.GetCurrentUnixTime();

    _dbContext.UserSessions.Update(session);
    _dbContext.SaveChanges();
  }

  /// <summary>
  /// Record a node play on the session
  /// </summary>
  /// <param name="nodeId">Node Id</param>
  public void OnPlayNode(MapsNodesFullRelationsDto dto)
  {
    Guard.Argument(_mapId, nameof(_mapId)).Positive();

    var nodeId = dto.Id.Value;
    _logger.LogInformation($"OnPlayNode: session {GetSessionId()} Map: {_mapId} Node: {nodeId}");

    var session = GetSessionFromDatabase(GetSessionId());
    if (session == null)
      return;

    // abbreviate counter dto's into shorter version dto
    var countersDto = dto.DynamicObjects.ToCounterValues();
    var counterJson = JsonSerializer.Serialize(countersDto);

    var sessionTrace = new UserSessiontraces
    {
      SessionId = session.Id,
      MapId = _mapId,
      NodeId = nodeId,
      DateStamp = Conversions.GetCurrentUnixTime(),
      UserId = session.UserId
    };

    _dbContext.UserSessiontraces.Add(sessionTrace);
    _dbContext.SaveChanges();

    var counterUpdate = new UserCounterUpdate
    {
      CounterState = counterJson
    };

    _dbContext.UserCounterUpdate.Add(counterUpdate);
    _dbContext.SaveChanges();

    // hook up session trace to counter update

    var userSessionTraceCounterUpdate = new UsersessiontraceCounterupdate
    {
      CounterupdateId = counterUpdate.Id,
      SessiontraceId = sessionTrace.Id
    };

    _dbContext.UsersessiontraceCounterupdate.Add(userSessionTraceCounterUpdate);
    _dbContext.SaveChanges();

  }

  public void OnQuestionResponse(
    QuestionResponsePostDataDto body,
    SystemQuestions questionPhys)
  {
    Guard.Argument(_mapId, nameof(_mapId)).Positive();
    Guard.Argument(body, nameof(body)).NotNull();
    Guard.Argument(questionPhys, nameof(questionPhys)).NotNull();

    var sessionPhys = GetSessionFromDatabase(GetSessionId());
    if (sessionPhys == null)
      return;

    _logger.LogInformation($"OnQuestionResponse: session {GetSessionId()} Map: {_mapId} Node: {body.NodeId} Question: {questionPhys.Id} = {body.Value} ");

    // truncate the message in case it's too long
    if (string.IsNullOrEmpty(body.Value) && (body.Value.Length > 1000))
      body.Value = body.Value[997..] + "...";

    // abbreviate counter dto's into shorter version dto
    var countersDto = body.DynamicObjects.ToCounterValues();
    var counterJson = JsonSerializer.Serialize(countersDto);

    // save the response and the associated counter dump

    var userResponse = new UserResponses
    {
      SessionId = sessionPhys.Id,
      QuestionId = questionPhys.Id,
      Response = body.Value,
      NodeId = body.NodeId,
      CreatedAt = Conversions.GetCurrentUnixTime()
    };

    _dbContext.UserResponses.Add(userResponse);
    _dbContext.SaveChanges();

    var counterUpdate = new UserCounterUpdate
    {
      CounterState = counterJson
    };

    _dbContext.UserCounterUpdate.Add(counterUpdate);
    _dbContext.SaveChanges();

    // hook up user response to counter update

    var userResponseCounterUpdate = new UserresponseCounterupdate
    {
      CounterupdateId = counterUpdate.Id,
      UserresponseId = userResponse.Id
    };

    _dbContext.UserresponseCounterupdate.Add(userResponseCounterUpdate);
    _dbContext.SaveChanges();

    _logger.LogInformation($"OnQuestionResponse: saved user response to session");


  }

  /// <summary>
  /// Record a question response on the session
  /// </summary>
  /// <param name="nodeId">Node Id</param>
  /// <param name="questionId">Question Id</param>
  /// <param name="value">Question response value</param>
  //public UserResponses OnQuestionResponse(uint nodeId, uint questionId, string value)
  //{
  //  Guard.Argument(_mapId, nameof(_mapId)).Positive();
  //  Guard.Argument(nodeId, nameof(nodeId)).Positive();
  //  Guard.Argument(questionId, nameof(questionId)).Positive();

  //  var physSession = GetSessionFromDatabase(GetSessionId());
  //  if (physSession == null)
  //    return null;

  //  _logger.LogInformation($"OnQuestionResponse: session {GetSessionId()} Map: {_mapId} Node: {nodeId} Question: {questionId} = {value} ");

  //  // truncate the message in case it's too long
  //  if (string.IsNullOrEmpty(value) && (value.Length > 1000))
  //    value = value[997..] + "...";

  //  var userResponse = new UserResponses
  //  {
  //    SessionId = physSession.Id,
  //    QuestionId = questionId,
  //    Response = value,
  //    NodeId = nodeId,
  //    CreatedAt = Conversions.GetCurrentUnixTime()
  //  };

  //  _dbContext.UserResponses.Add(userResponse);
  //  _dbContext.SaveChanges();

  //  _logger.LogInformation($"OnQuestionResponse: saved user response to session");

  //  return userResponse;
  //}

  public void SaveSessionState(uint nodeId, DynamicScopedObjectsDto dynamicObjects)
  {
    Guard.Argument(_mapId, nameof(_mapId)).Positive();

    var userState = new UserState
    {
      MapId = _mapId,
      MapNodeId = nodeId,
      UserId = _userContext.UserId,
      StateData = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(dynamicObjects))
    };

    _dbContext.UserState.Add(userState);
    _dbContext.SaveChanges();
  }

  /// <summary>
  /// Retrieve session database record
  /// </summary>
  /// <param name="sessionId">Session Id</param>
  /// <returns></returns>
  private UserSessions GetSessionFromDatabase(string sessionId)
  {
    Guard.Argument(sessionId).NotNull(nameof(sessionId));

    var physSession = _dbContext.UserSessions.FirstOrDefault(x => x.Uuid == sessionId);
    if (physSession == null)
    {
      _logger.LogError($"Unable to get session, sessionId '{sessionId}' not found");
      return null;
    }

    return physSession;
  }

}