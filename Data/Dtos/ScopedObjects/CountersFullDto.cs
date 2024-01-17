using Newtonsoft.Json;

namespace OLab.Data.Dtos;

public class CountersFullDto : CountersDto
{
  [JsonProperty("startValue")]
  public string StartValue { get; set; }
  [JsonProperty("iconId")]
  public int? IconId { get; set; }
  [JsonProperty("prefix")]
  public string Prefix { get; set; }
  [JsonProperty("suffix")]
  public string Suffix { get; set; }
  [JsonProperty("visible")]
  public bool? Visible { get; set; }
  [JsonProperty("outOf")]
  public int? OutOf { get; set; }
  [JsonProperty("status")]
  public int Status { get; set; }
}