using Newtonsoft.Json;


namespace OLab.Data.Dtos;

public class MapsScopedObjectsDto : MapsDto
{
  [JsonProperty("scopedObjects")]
  public ScopedObjectsDto ScopedObjects { get; set; }

}
