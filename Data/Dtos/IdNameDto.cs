using Newtonsoft.Json;

namespace OLab.Api.Dto;

public class IdNameDto
{
  [JsonProperty("id")]
  public uint Id { get; set; }
  [JsonProperty("name")]
  public string Name { get; set; }

  public override string ToString()
  {
    return $" '{Name}({Id})'";
  }
}