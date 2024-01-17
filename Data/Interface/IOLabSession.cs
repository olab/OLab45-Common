
using OLab.Data.Dtos;
using System;

namespace OLab.Data.Interface;

public interface IOLabSession
{
  public void OnStartSession(IUserContext userContext, uint mapId);
  public void OnPlayNode(uint mapId, uint nodeId);
  public void OnQuestionResponse(uint mapId, uint nodeId, uint questionId, string value);
  public void OnExtendSession(uint mapId, uint nodeId);
  public string GetSessionId();
  public void SetSessionId(string sessionId);
  public void SaveSessionState(uint mapId, uint nodeId, DynamicScopedObjectsDto dynamicObjects);

  public static string GenerateSessionId()
  {
    var sessionId = Guid.NewGuid().ToString();
    return sessionId;
  }
}