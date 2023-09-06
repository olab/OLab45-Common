using Newtonsoft.Json;

namespace OLab.Dto
{
  public class ConstantsDto : ScopedObjectDto
  {
    [JsonProperty("value")]
    public string Value { get; set; }
    [JsonProperty("isSystem")]
    public int? IsSystem { get; set; }
  }
}