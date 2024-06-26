using Newtonsoft.Json;

namespace OLab.Api.Dto;

public class MapNodesPostDataDto
{
  [JsonProperty("sourceId")]
  public uint SourceId { get; set; }
  [JsonProperty("x")]
  public double X { get; set; }
  [JsonProperty("y")]
  public double Y { get; set; }
}