using Newtonsoft.Json;

namespace OLab.Data.Dtos
{
  public class MapNodeLinksPostResponseDto
  {
    [JsonProperty("id")]
    public uint Id { get; set; }
  }
}
