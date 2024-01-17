using Newtonsoft.Json;

namespace OLab.Data.Dtos;

public class CounterActionsDto
{
  [JsonProperty("id")]
  public uint Id { get; set; }
  [JsonProperty("nodeId")]
  public uint NodeId { get; set; }
  [JsonProperty("counterId")]
  public uint CounterId { get; set; }
  [JsonProperty("function")]
  public string Function { get; set; }
  [JsonProperty("display")]
  public int Display { get; set; }

  public override string ToString()
  {
    return $" '{Function}({Id})'";
  }
}
