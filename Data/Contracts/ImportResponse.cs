using OLab.Api.Utils;
using System.Collections.Generic;

namespace OLab.Data.Contracts
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