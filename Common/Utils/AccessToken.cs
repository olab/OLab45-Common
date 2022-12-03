using Microsoft.AspNetCore.Http;
using OLabWebAPI.Common.Exceptions;

namespace OLabWebAPI.Utils
{
    public static class AccessTokenUtils
    {
        public static string ExtractBearerToken(HttpRequest request)
        {
            if (!request.Headers.ContainsKey("Authorization"))
                throw new OLabUnauthorizedException();

            string authorizationHeader = request.Headers["Authorization"];
            // Check if the value is empty.
            if (string.IsNullOrEmpty(authorizationHeader))
                throw new OLabUnauthorizedException();

            string[] parts = authorizationHeader.Split(" ");
            if (parts.Length != 2)
                throw new OLabUnauthorizedException();

            string accessToken = parts[1];
            return accessToken;
        }
    }
}