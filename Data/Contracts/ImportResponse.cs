using OLab.Utils;
using System.Collections.Generic;

namespace OLab.Model
{
  public class ImportResponse
  {
    public IList<OLabLogMessage> Messages { get; set; }

    public ImportResponse()
    {
      Messages = new List<OLabLogMessage>();
    }
  }
}