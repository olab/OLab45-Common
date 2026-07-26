using Newtonsoft.Json;

namespace OLab.Api.Dto;

public class MapNodeLinksDto
{
  [JsonProperty( "id" )]
  public uint? Id { get; set; }
  [JsonProperty( "sourceId" )]
  public uint? SourceId { get; set; }
  [JsonProperty( "destinationId" )]
  public uint? DestinationId { get; set; }
  [JsonProperty( "color" )]
  public string Color { get; set; }
  [JsonProperty( "reverseId" )]
  public uint? ReverseId { get; set; }
  [JsonProperty( "linkText" )]
  public string LinkText { get; set; }
  [JsonProperty( "destinationTitle" )]
  public string DestinationTitle { get; set; }
  [JsonProperty( "linkStyleId" )]
  public uint? LinkStyleId { get; set; }
  [JsonProperty( "isHidden" )]
  public bool IsHidden { get; internal set; }
  [JsonProperty( "followOnce" )]
  public bool FollowOnce { get; internal set; }
  [JsonProperty( "type" )]
  public string ObjectType = "link";
  public override string ToString()
  {
    return $"{Id} {SourceId} -> {DestinationTitle}({DestinationId})";
  }
}
