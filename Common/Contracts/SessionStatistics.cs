using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLab.Common.Contracts;
public class SessionStatistics
{
  public DateTime? SessionStart { get; set; }
  public DateTime? SessionEnd { get; set; }
  public TimeSpan SessionDuration{ get; set; }
  public int NodeCount { get; set; }
}
