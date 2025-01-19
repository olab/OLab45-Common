using Newtonsoft.Json;
using System.Collections.Generic;

namespace OLab.Api.Common;

public class OLabAPIPagedResponse<D> : OLabApiResult<IList<D>>
{
  [JsonProperty("count")]
  public int Count { get; set; }
  [JsonProperty("remaining")]
  public int Remaining { get; set; }

  public OLabAPIPagedResponse()
  {
    Count = 0;
    Remaining = 0;
  }
}
