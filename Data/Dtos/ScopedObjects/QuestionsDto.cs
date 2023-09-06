using Newtonsoft.Json;

namespace OLab.Dto
{
  public class QuestionsDto : ScopedObjectDto
  {
    [JsonProperty("questionType")]
    public uint EntryTypeId { get; set; }
  }
}