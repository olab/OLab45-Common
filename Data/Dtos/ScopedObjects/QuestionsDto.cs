using Newtonsoft.Json;

namespace OLab.Data.Dtos
{
  public class QuestionsDto : ScopedObjectDto
  {
    [JsonProperty("questionType")]
    public uint EntryTypeId { get; set; }
  }
}