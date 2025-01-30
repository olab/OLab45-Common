using Newtonsoft.Json;

namespace OLab.Api.Dto;

public class ConstantsDto : ScopedObjectDto
{
  [JsonProperty( "value" )]
  public string Value { get; set; }
  [JsonProperty( "isSystem" )]
  public int? IsSystem { get; set; }

  public ConstantsDto()
  {
    ObjectType = "constant";

  }
  public override string ToString()
  {
    return $" '{Name}({Id})' = {Value}";
  }
}