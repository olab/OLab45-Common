using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace OLab.Common
{
  public class Diagnostics
  {
    public Diagnostics()
    {
    }

    [JsonProperty("stack")]
    public string Stack { get; set; }
    [JsonProperty("file")]
    public string File { get; set; }
    [JsonProperty("line")]
    public string Line { get; set; }
  }

  public class OLabAPIResponse<D> : ActionResult
  {
    public const string MessageSuccess = "success";

    public OLabAPIResponse()
    {
      Diagnostics = new List<Diagnostics>();
      Message = MessageSuccess;
      ErrorCode = HttpStatusCode.OK;
    }

    [JsonProperty("extended_status_code")]
    public int? Status { get; set; }
    [JsonProperty("message")]
    public string Message { get; set; }
    [JsonProperty("error_code")]
    public HttpStatusCode ErrorCode { get; set; }
    [JsonProperty("diagnostics")]
    public IList<Diagnostics> Diagnostics { get; set; }
    [JsonProperty("data")]
    public D Data { get; set; }
  }
}
