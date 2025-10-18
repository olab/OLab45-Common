using Newtonsoft.Json;
using System;

namespace OLab.Api.Model;

public class AuthenticateResponse
{
  [JsonProperty( "createdAt" )]
  public DateTime CreatedAt { get; set; }
  [JsonProperty( "userName" )]
  public string UserName { get; set; }
  [JsonProperty( "role" )]
  public string Role { get; set; }
  [JsonProperty( "authInfo" )]
  public RefreshToken AuthInfo { get; set; }
  [JsonProperty( "courseName" )]
  public string CourseName { get; set; }

  public AuthenticateResponse()
  {
    CreatedAt = DateTime.Now;
    AuthInfo = new RefreshToken();
  }

}