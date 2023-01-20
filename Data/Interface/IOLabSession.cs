using System;

namespace OLabWebAPI.Data.Interface
{
    public interface IOLabSession
    {
        public void OnStartSession(IUserContext userContext, uint mapId);
        public void OnPlayNode(uint mapId, uint nodeId);
        public void OnQuestionResponse(uint mapId, uint nodeId, uint questionId, string value);
        public void OnEndSession(uint mapId, uint nodeId);
        public string GetSessionId();
        public void SetSessionId(string sessionId);

        public static string GenerateSessionId()
        {
            var sessionId = Guid.NewGuid().ToString();
            return sessionId;
        }
    }
}