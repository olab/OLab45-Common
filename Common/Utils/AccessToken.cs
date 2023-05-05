using Microsoft.AspNetCore.Http;
using OLabWebAPI.Common.Exceptions;

namespace OLabWebAPI.Utils
{
  public static class AccessTokenUtils
  {
    public static string ExtractAccessToken(HttpRequest request, bool allowAnonymous = false)
    {
      var token = "";

      if (request.Headers.ContainsKey("Authorization"))
      {
        token = request.Headers["Authorization"];

        var parts = token.Split(" ");
        if (parts.Length != 2)
          throw new OLabUnauthorizedException();


        token = parts[1];
      }

      // handler external app posted token
      if (request.Query.ContainsKey("token"))
        token = request.Query["token"];

      // handler SignalR posted token
      if (request.Query.ContainsKey("access_token"))
        token = request.Query["access_token"];

      if (string.IsNullOrEmpty(token) && !allowAnonymous)
        throw new OLabUnauthorizedException();

      return token;
    }
  }
}