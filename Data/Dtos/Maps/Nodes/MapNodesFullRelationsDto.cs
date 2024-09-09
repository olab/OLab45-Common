using Newtonsoft.Json;
using System.Collections.Generic;

namespace OLab.Api.Dto;

public class MapsNodesFullRelationsDto : MapNodesFullDto
{
  [JsonProperty("groupRoles")]
  public IList<MapNodeGroupRolesDto> MapNodeGrouproles { get; set; }

  [JsonProperty("links")]
  public IList<MapNodeLinksDto> MapNodeLinks { get; set; }

  [JsonProperty("dynamicObjects")]
  public DynamicScopedObjectsDto DynamicObjects { get; set; }

  public int LinkCount { get { return MapNodeLinks.Count; } }

  public MapsNodesFullRelationsDto()
  {
    MapNodeGrouproles = new List<MapNodeGroupRolesDto>();
    MapNodeLinks = new List<MapNodeLinksDto>();
    DynamicObjects = new DynamicScopedObjectsDto();
  }

}
