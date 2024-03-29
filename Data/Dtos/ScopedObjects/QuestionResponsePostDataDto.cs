using Newtonsoft.Json;

namespace OLab.Api.Dto;

public class QuestionResponsePostDataDto
{
  [JsonProperty("questionId")]
  public uint QuestionId { get; set; }
  [JsonProperty("previousResponseId")]
  public uint? PreviousResponseId { get; set; }
  [JsonProperty("responseId")]
  public uint? ResponseId { get; set; }
  [JsonProperty("previousValue")]
  public string PreviousValue { get; set; }
  [JsonProperty("value")]
  public string Value { get; set; }
  [JsonProperty("mapid")]
  public uint MapId { get; set; }
  [JsonProperty("NodeId")]
  public uint NodeId { get; set; }
  [JsonProperty("dynamicObjects")]
  public DynamicScopedObjectsDto DynamicObjects { get; set; }
}