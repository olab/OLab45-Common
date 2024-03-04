using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OLab.Common.Contracts;
public class HealthResult
{
  public HttpStatusCode statusCode;
  public System.Version main;
  public object modules;
  public string message;
}