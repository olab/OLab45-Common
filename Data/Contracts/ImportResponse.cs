using OLabWebAPI.Utils;
using System.Collections.Generic;

namespace OLabWebAPI.Model
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