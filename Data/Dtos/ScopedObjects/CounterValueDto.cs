using Newtonsoft.Json;
using OLab.Api.Dto;

namespace OLab.Data.Dtos.ScopedObjects;
public class CounterValueDto
{
  [JsonProperty("id")]
  public uint? Id { get; set; }
  [JsonProperty("name")]
  public string Name { get; set; }
  [JsonProperty("value")]
  public string Value { get; set; }

  public CounterValueDto(CountersDto sourceDto)
  {
    Id = sourceDto.Id;
    Name = sourceDto.Name;
    Value = sourceDto.Value;
  }
}
