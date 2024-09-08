using Newtonsoft.Json;

namespace OLab.Api.Dto;

public partial class MapGroupsDto
{
  [JsonProperty("mapId")]
  public uint MapId { get; set; }
  [JsonProperty("id")]
  public uint GroupId { get; set; }
  [JsonProperty("name")]
  public string GroupName { get; set; }
}
