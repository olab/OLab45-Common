using Dawn;
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

  public OLabSession(
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
    var countersDto = new List<CounterValueDto>();
    foreach (var counterDto in dto.DynamicObjects.Server.Counters)
      countersDto.Add(new CounterValueDto(counterDto));
    foreach (var counterDto in dto.DynamicObjects.Map.Counters)
      countersDto.Add(new CounterValueDto(counterDto)); 
    foreach (var counterDto in dto.DynamicObjects.Node.Counters)
      countersDto.Add(new CounterValueDto(counterDto));

    var counterJson = JsonSerializer.Serialize(countersDto);

    var sessionTrace = new UserSessiontraces
    {
      SessionId = session.Id,
      MapId = _mapId,
      NodeId = nodeId,
      DateStamp = Conversions.GetCurrentUnixTime(),
      UserId = session.UserId,
      Counters = counterJson
    };

    _dbContext.UserSessiontraces.Add(sessionTrace);
    _dbContext.SaveChanges();

  }

  /// <summary>
  /// Record a question response on the session
  /// </summary>
  /// <param name="nodeId">Node Id</param>
  /// <param name="questionId">Question Id</param>
  /// <param name="value">Question response value</param>
  public void OnQuestionResponse(uint nodeId, uint questionId, string value)
  {
    Guard.Argument(_mapId, nameof(_mapId)).Positive();
    Guard.Argument(nodeId, nameof(nodeId)).Positive();
    Guard.Argument(questionId, nameof(questionId)).Positive();

    var physSession = GetSessionFromDatabase(GetSessionId());
    if (physSession == null)
      return;

    _logger.LogInformation($"OnQuestionResponse: session {GetSessionId()} Map: {_mapId} Node: {nodeId} Question: {questionId} = {value} ");

    // truncate the message in case it's too long
    if (string.IsNullOrEmpty(value) && (value.Length > 1000))
      value = value[997..] + "...";

    var userResponse = new UserResponses
    {
      SessionId = physSession.Id,
      QuestionId = questionId,
      Response = value,
      NodeId = nodeId,
      CreatedAt = Conversions.GetCurrentUnixTime()
    };

    _dbContext.UserResponses.Add(userResponse);
    _dbContext.SaveChanges();

    _logger.LogInformation($"OnQuestionResponse: saved user response to session");
  }

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