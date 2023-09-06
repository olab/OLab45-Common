using Newtonsoft.Json;

namespace OLab.Api.Dto
{
  public class MapNodeLinksPostDataDto
  {
    [JsonProperty("destinationId")]
    public uint DestinationId { get; set; }
  }
}