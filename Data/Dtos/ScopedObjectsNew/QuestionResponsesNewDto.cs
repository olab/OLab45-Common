using Newtonsoft.Json;
using OLabWebAPI.Model;
using OLabWebAPI.Utils;
using System;

namespace OLabWebAPI.Dto
{
  public class QuestionResponsesNewDto
  {
    protected readonly QuestionsFullNewDto ParentQuestion;

    public QuestionResponsesNewDto()
    {
      ParentInfo = new IdName();
    }

    [JsonProperty("id")]
    public uint? Id { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("parentId")]
    public uint ImageableId { get; set; }
    [JsonProperty("scopeLevel")]
    public string ImageableType { get; set; }
    [JsonProperty("createdat")]
    public DateTime? CreatedAt { get; set; }
    [JsonProperty("updatedat")]
    public DateTime? UpdatedAt { get; set; }
    [JsonProperty("scopeLevelObj")]
    public IdName ParentInfo { get; set; }

    [JsonProperty("response")]
    public string Response { get; set; }
    [JsonProperty("feedback")]
    public string Feedback { get; set; }
    [JsonProperty("isCorrect")]
    public int? IsCorrect { get; set; }
    [JsonProperty("score")]
    public int? Score { get; set; }
    [JsonProperty("from")]
    public string From { get; set; }
    [JsonProperty("to")]
    public string To { get; set; }
    [JsonProperty("order")]
    public uint Order { get; set; }
    [JsonProperty("questionId")]
    public uint QuestionId { get; set; }

    // calculated properties
    [JsonProperty("value")]
    public object Value { get; set; }
  }
}