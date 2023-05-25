using Mapster;
using Newtonsoft.Json;
using OLabWebAPI.Model;
using OLabWebAPI.Utils;
using System;
using System.Collections.Generic;

namespace OLabWebAPI.Dto
{
  public class QuestionsFullNewDto
  {
    public QuestionsFullNewDto()
    {
      Responses = new List<QuestionResponsesNewDto>();
      ParentInfo = new IdName();
    }

    public static TypeAdapterConfig GetMapsterConfig()
    {
      return new TypeAdapterConfig()
        .NewConfig<SystemQuestions, QuestionsFullNewDto>()
        .Map(dest => dest.Responses, src => src.SystemQuestionResponses.Adapt<IList<QuestionResponsesNewDto>>())
        //.Map(dest => dest.Value, null)
        .Config;
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

    [JsonProperty("stem")]
    public string Stem { get; set; }
    [JsonProperty("width")]
    public int Width { get; set; }
    [JsonProperty("height")]
    public int Height { get; set; }
    [JsonProperty("settings")]
    public string Settings { get; set; }
    [JsonProperty("feedback")]
    public string Feedback { get; set; }
    [JsonProperty("prompt")]
    public string Prompt { get; set; }
    [JsonProperty("showAnswer")]
    public bool? ShowAnswer { get; set; }
    [JsonProperty("showSubmit")]
    public sbyte ShowSubmit { get; set; }
    [JsonProperty("layoutType")]
    public int TypeDisplay { get; set; }
    [JsonProperty("isPrivate")]
    public int IsPrivate { get; set; }
    [JsonProperty("order")]
    public int? Order { get; set; }
    [JsonProperty("responses")]
    public List<QuestionResponsesNewDto> Responses { get; set; }

    // calculated/transient state fields
    [JsonProperty("tryCount")]
    public int TryCount { get; set; }
    [JsonProperty("numTries")]
    public int NumTries { get; set; }
    [JsonProperty("disabled")]
    public int Disabled { get; set; }

    // calculated properties
    [JsonProperty("wiki")]
    public string Wiki { get; set; }
    [JsonProperty("value")]
    public string Value { get; set; }

    [JsonProperty("questionType")]
    public uint EntryTypeId { get; set; }

  }
}