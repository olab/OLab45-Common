using System;

namespace OLab.Data.Dtos.Session;

public class UserSessionTracesDto
{
  public bool IsRedirected { get; set; }
  public DateTime DateStamp { get; set; }
  public DateTime? EndDateStamp { get; set; }
  public short? Confidence { get; set; }
  public string Counters { get; set; }
  public string Dams { get; set; }
  public uint Id { get; set; }
  public uint MapId { get; set; }
  public uint NodeId { get; set; }
  public uint UserId { get; set; }
}