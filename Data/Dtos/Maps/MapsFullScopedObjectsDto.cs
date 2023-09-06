using Newtonsoft.Json;

namespace OLab.Dto
{
  public class MapsScopedObjectsDto : MapsDto
  {
    [JsonProperty("scopedObjects")]
    public ScopedObjectsDto ScopedObjects { get; set; }

  }
}
