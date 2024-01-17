using Newtonsoft.Json;

namespace OLab.Data.Dtos;

public class ConstantsDto : ScopedObjectDto
{
  [JsonProperty("value")]
  public string Value { get; set; }
  [JsonProperty("isSystem")]
  public int? IsSystem { get; set; }

  public override string ToString()
  {
    return $" '{Name}({Id})' = {Value}";
  }
}