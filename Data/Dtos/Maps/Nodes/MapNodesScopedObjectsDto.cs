using Newtonsoft.Json;

namespace OLab.Dto
{
  public class MapNodesScopedObjectsDto : MapNodesDto
  {
    [JsonProperty("scopedObjects")]
    public ScopedObjectsDto ScopedObjects { get; set; }

  }
}
