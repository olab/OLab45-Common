using OLabWebAPI.Model;
using System;

namespace OLabWebAPI.Data.Interface
{
  public interface IOLabSession
  {
    public void OnStartSession(IUserContext userContext, uint mapId);
    public void OnPlayNode(uint mapId, uint nodeId);
    public void OnQuestionResponse(uint mapId, uint nodeId, SystemQuestions question, string value);
    public void OnEndSession(uint mapId, uint nodeId);
    public string GetSessionId();
    public void SetSessionId(string sessionId);

    public static string GenerateSessionId()
    {
      string sessionId = Guid.NewGuid().ToString();
      return sessionId;
    }
  }
}