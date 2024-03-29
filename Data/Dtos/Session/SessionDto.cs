using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable disable

namespace OLab.Data.Dtos.Session;

public partial class SessionDto
{
  public SessionDto()
  {
    StatementsDtos = new HashSet<StatementsDto>();
    UserResponsesDtos = new HashSet<UserResponsesDto>();
    UserSessionTracesDtos = new HashSet<UserSessionTracesDto>();
  }

  public uint Id { get; set; }
  public string Uuid { get; set; }
  [JsonProperty("Issuer")]
  public string Iss { get; set; }
  public uint UserId { get; set; }
  public uint MapId { get; set; }
  public DateTime StartTime { get; set; }
  public DateTime? EndTime { get; set; }

  [JsonProperty("Statements")]
  public HashSet<StatementsDto> StatementsDtos { get; set; }
  [JsonProperty("Responses")]
  public HashSet<UserResponsesDto> UserResponsesDtos { get; set; }
  [JsonProperty("Activity")]
  public HashSet<UserSessionTracesDto> UserSessionTracesDtos { get; set; }
}
