using Newtonsoft.Json;

namespace OLab.Api.Dto;

public class MapNodeLinksPostResponseDto
{
  [JsonProperty("id")]
  public uint Id { get; set; }
}
