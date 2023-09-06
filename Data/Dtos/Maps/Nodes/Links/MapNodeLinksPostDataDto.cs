using Newtonsoft.Json;

namespace OLab.Dto
{
  public class MapNodeLinksPostDataDto
  {
    [JsonProperty("destinationId")]
    public uint DestinationId { get; set; }
  }
}