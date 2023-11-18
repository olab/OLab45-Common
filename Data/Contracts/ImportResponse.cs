using OLab.Api.Utils;
using System.Collections.Generic;

namespace OLab.Api.Model
{
  public class ImportResponse
  {
    //public IList<OLabLogMessage> Objects { get; set; }
    public IList<string> Messages { get; set; }

    public ImportResponse()
    {
      //Objects = new List<OLabLogMessage>();
      Messages = new List<string>();
    }

  }
}