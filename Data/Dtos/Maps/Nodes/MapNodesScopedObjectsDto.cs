using Newtonsoft.Json;

namespace OLab.Api.Dto
{
  public class MapNodesScopedObjectsDto : MapNodesDto
  {
    [JsonProperty("scopedObjects")]
    public ScopedObjectsDto ScopedObjects { get; set; }

  }
}
