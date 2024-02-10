using OLab.Api.Dto;
using System;

namespace OLab.Api.Data.Interface
{
  public interface IOLabSession
  {
    public void SetMapId(uint mapId);
    public void OnStartSession();
    public void OnPlayNode(uint nodeId);
    public void OnQuestionResponse(uint nodeId, uint questionId, string value);
    public void OnExtendSessionEnd(uint nodeId);
    public string GetSessionId();
    public void SetSessionId(string sessionId);
    public void SaveSessionState(uint nodeId, DynamicScopedObjectsDto dynamicObjects);

    public static string GenerateSessionId()
    {
      var sessionId = Guid.NewGuid().ToString();
      return sessionId;
    }
  }
}