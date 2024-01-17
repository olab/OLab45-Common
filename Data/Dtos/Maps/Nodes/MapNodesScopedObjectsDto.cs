using Newtonsoft.Json;


namespace OLab.Data.Dtos;

public class MapNodesScopedObjectsDto : MapNodesDto
{
  [JsonProperty("scopedObjects")]
  public ScopedObjectsDto ScopedObjects { get; set; }

}
