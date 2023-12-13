using Newtonsoft.Json;

namespace OLab.Data.Dtos
{
  public class MapNodeLinksPostDataDto
  {
    [JsonProperty("destinationId")]
    public uint DestinationId { get; set; }
  }
}