using System.Collections.Generic;
using System.Net;

namespace OLab.Common.Contracts;

public class HealthResult
{
  public HttpStatusCode StatusCode;
  public System.Version Main;
  public IDictionary<string, string> Modules;
  public string Message;
}