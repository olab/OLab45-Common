using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLabWebAPI.Model
{
  public partial class SessionInfo
  {
    public string uuid { get; set; }
    public DateTime Timestamp { get; set; }
    public string User { get; set; }
    public uint NodesVisited { get; set; }
  }
}
