using Newtonsoft.Json;
using System.Collections.Generic;

namespace OLabWebAPI.Common
{
    public class OLabAPIPagedResponse<D> : OLabAPIResponse<IList<D>>
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("remaining")]
        public int Remaining { get; set; }
    }
}
