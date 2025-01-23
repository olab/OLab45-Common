using Newtonsoft.Json;

namespace OLab.Api.Dto.Designer;

public class ScopeLevelObj
{
  [JsonProperty( "id" )]
  public uint Id { get; set; }
  [JsonProperty( "name" )]
  public string Name { get; set; }
  [JsonProperty( "description" )]
  public string Description { get; set; }
}