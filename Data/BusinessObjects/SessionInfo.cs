using System;

#nullable disable

namespace OLab.Api.Model;

public partial class SessionInfo
{
  public string uuid { get; set; }
  public DateTime Timestamp { get; set; }
  public string User { get; set; }
  public uint NodesVisited { get; set; }
}
