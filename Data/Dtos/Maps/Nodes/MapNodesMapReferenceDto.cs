using Newtonsoft.Json;

namespace OLab.Api.Dto;

public class MapNodesMapReferenceDto : MapNodesDto
{
  public uint MapId { get; set; }
  public string MapName { get; set; }
}
