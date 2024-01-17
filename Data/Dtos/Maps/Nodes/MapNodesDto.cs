using Newtonsoft.Json;

namespace OLab.Data.Dtos;

public class MapNodesDto
{
  [JsonProperty("contextId")]
  public string ContextId { get; set; }
  [JsonProperty("title")]
  public string Title { get; set; }
  [JsonProperty("url")]
  public string Url { get; set; }
  [JsonProperty("id")]
  public uint? Id { get; set; }
}
