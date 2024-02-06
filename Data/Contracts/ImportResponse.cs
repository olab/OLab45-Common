using System.Collections.Generic;

namespace OLab.Data.Contracts;

public class ImportResponse
{
  public IList<string> LogMessages { get; set; }
  public uint MapId { get; set; }

  public ImportResponse()
  {
    LogMessages = new List<string>();
  }
}