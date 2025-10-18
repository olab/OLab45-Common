using Newtonsoft.Json;
using System;

namespace OLab.Api.Model;

public class RefreshToken
{
  [JsonProperty( "token" )]
  public string Token { get; set; }
  [JsonProperty( "refresh" )]
  public string Refresh { get; set; }
  [JsonProperty( "expires" )]
  public DateTime Expires { get; set; }
  [JsonProperty( "isExpired" )]
  public bool IsExpired => DateTime.UtcNow >= Expires;
  [JsonProperty( "created" )]
  public DateTime Created { get; set; }
  [JsonProperty( "revoked" )]
  public DateTime? Revoked { get; set; }
  [JsonProperty( "isActive" )]
  public bool IsActive => Revoked == null && !IsExpired;
}