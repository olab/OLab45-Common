using Newtonsoft.Json;

namespace OLab.Api.Dto;

public class QuestionsDto : ScopedObjectDto
{
  [JsonProperty("questionType")]
  public uint EntryTypeId { get; set; }
}