using OLab.Api.Utils;
using System.Collections.Generic;

namespace OLab.Api.Model;

public class ImportResponse
{
  public IList<OLabLogMessage> LogMessages { get; set; }
  public uint MapId { get; set; }

  public ImportResponse()
  {
    LogMessages = new List<OLabLogMessage>();
  }
}