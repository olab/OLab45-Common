using Newtonsoft.Json;

namespace OLab.Api.Dto;

public class MapsScopedObjectsDto : MapsDto
{
  [JsonProperty("scopedObjects")]
  public ScopedObjectsDto ScopedObjects { get; set; }

}
