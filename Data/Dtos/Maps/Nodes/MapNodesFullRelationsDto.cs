using Newtonsoft.Json;
using System.Collections.Generic;

namespace OLab.Api.Dto;

public class MapsNodesFullRelationsDto : MapNodesFullDto
{
  [JsonProperty( "links" )]
  public IList<MapNodeLinksDto> MapNodeLinks { get; set; }

  [JsonProperty( "dynamicObjects" )]
  public DynamicScopedObjectsDto DynamicObjects { get; set; }

  public int LinkCount { get { return MapNodeLinks.Count; } }

  public MapsNodesFullRelationsDto()
  {
    MapNodeLinks = new List<MapNodeLinksDto>();
    DynamicObjects = new DynamicScopedObjectsDto();
  }

}
