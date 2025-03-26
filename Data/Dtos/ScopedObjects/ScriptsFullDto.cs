using Newtonsoft.Json;

namespace OLab.Api.Dto;

public class ScriptsFullDto : ScriptsDto
{
  [JsonProperty( "order" )]
  public int? Order { get; set; }

  // calculated properties
  [JsonProperty( "wiki" )]
  public string Wiki { get; set; }
  public string OriginUrl { get; set; }
  public string HostName { get; set; }
}