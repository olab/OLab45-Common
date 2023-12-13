using Newtonsoft.Json;

namespace OLab.Data.Dtos
{
  public class MapNodeCountersDto
  {
    [JsonProperty("id")]
    public uint Id { get; set; }
    [JsonProperty("nodeId")]
    public uint NodeId { get; set; }
    [JsonProperty("counterId")]
    public int CounterId { get; set; }
    [JsonProperty("function")]
    public string Function { get; set; }
    [JsonProperty("display")]
    public int Display { get; set; }
  }
}
