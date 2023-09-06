using Newtonsoft.Json;

namespace OLab.Api.Dto
{
  public class MapNodeLinksFullDto : MapNodeLinksDto
  {
    [JsonProperty("map_id")]
    public uint MapId { get; set; }
    [JsonProperty("node_id_1")]
    public uint NodeId1 { get; set; }
    [JsonProperty("node_id_2")]
    public uint NodeId2 { get; set; }
    [JsonProperty("image_id")]
    public uint? ImageId { get; set; }
    [JsonProperty("text")]
    public string Text { get; set; }
    [JsonProperty("order")]
    public int? Order { get; set; }
    [JsonProperty("probability")]
    public int? Probability { get; set; }
    [JsonProperty("hidden")]
    public bool? Hidden { get; set; }
    [JsonProperty("thickness")]
    public int? Thickness { get; set; }
    [JsonProperty("line_type")]
    public int? LineType { get; set; }
    [JsonProperty("follow_once")]
    public int? FollowOnce { get; set; }
  }
}
