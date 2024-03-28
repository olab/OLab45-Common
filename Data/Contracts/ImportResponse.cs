using OLab.Api.Utils;
using System;
using System.Collections.Generic;

namespace OLab.Api.Model;

public class ImportResponse
{
  public IList<string> LogMessages { get; set; }
  public uint Id { get; set; }
  public string Name { get; set; }
  public DateTime CreatedAt { get; set; }
  public IList<string> Groups { get; set; }

  public ImportResponse()
  {
    LogMessages = new List<string>();
    Groups = new List<string>();
  }
}