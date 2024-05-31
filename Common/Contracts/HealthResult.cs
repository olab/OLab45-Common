using System.Net;

namespace OLab.Common.Contracts;
public class HealthResult
{
  public HttpStatusCode statusCode;
  public System.Version main;
  public object modules;
  public string message;
}