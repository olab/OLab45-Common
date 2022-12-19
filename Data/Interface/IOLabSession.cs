using System;

namespace OLabWebAPI.Data.Interface
{
    public interface IOLabSession
    {
        public void OnStartSession(string userName, uint mapId, string ipAddress);
        public void OnPlayNode(uint mapId, uint nodeId);
        public void OnQuestionResponse(uint mapId, uint nodeId, uint questionId, string value);
        public void OnEndSession(uint mapId, uint nodeId);
        public string GetSessionId();
        public void SetSessionId( string sessionId );

        public static string GenerateSessionId()
        {
            string sessionId = Guid.NewGuid().ToString();
            return sessionId;
        }
    }
}