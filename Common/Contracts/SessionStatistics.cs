using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLab.Common.Contracts;
public class SessionStatistics
{
  public DateTime? SessionStart { get; set; } = DateTime.UtcNow;
  public DateTime? SessionEnd { get; set; } = null;
  public TimeSpan SessionDuration{ get; set; } = TimeSpan.Zero;
  public int NodeCount { get; set; } = 0;
  public string SessionId { get; set; } = "<unknown>";
}
