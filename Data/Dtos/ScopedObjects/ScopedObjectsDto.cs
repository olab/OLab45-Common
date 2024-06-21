using Newtonsoft.Json;
using OLab.Common.Interfaces;
using System.Collections.Generic;

namespace OLab.Api.Dto;

public class ScopedObjectsDto
{
  public ScopedObjectsDto()
  {
    Questions = new List<QuestionsFullDto>();
    Constants = new List<ConstantsDto>();
    Counters = new List<CountersDto>();
    Files = new List<FilesFullDto>();
    Scripts = new List<ScriptsFullDto>();
    Themes = new List<ThemesFullDto>();
    CounterActions = new List<CounterActionsDto>();
  }

  [JsonProperty("questions")]
  public List<QuestionsFullDto> Questions { get; set; }
  [JsonProperty("constants")]
  public List<ConstantsDto> Constants { get; set; }
  [JsonProperty("counters")]
  public List<CountersDto> Counters { get; set; }
  [JsonProperty("files")]
  public List<FilesFullDto> Files { get; set; }
  [JsonProperty("scripts")]
  public List<ScriptsFullDto> Scripts { get; set; }
  [JsonProperty("themes")]
  public List<ThemesFullDto> Themes { get; set; }
  [JsonProperty("counteractions")]
  public List<CounterActionsDto> CounterActions { get; set; }

  public void Dump(IOLabLogger logger)
  {
    logger.LogInformation("Result:");

    logger.LogInformation($" ConstantsPhys {Constants.Count}");
    logger.LogInformation($" QuestionsPhys {Questions.Count}");
    logger.LogInformation($" CounterMapper {Counters.Count}");
    logger.LogInformation($" FilesPhys {Files.Count}");
    logger.LogInformation($" ScriptsPhys {Scripts.Count}");
    logger.LogInformation($" ThemesPhys {Themes.Count}");
    logger.LogInformation($" CounterActionsPhys {CounterActions.Count}");

    foreach (var item in Constants)
      logger.LogInformation($" Constant {item}");

    foreach (var item in Questions)
      logger.LogInformation($" Question {item}");

    foreach (var item in Counters)
      logger.LogInformation($" Counter {item}");

    foreach (var item in Files)
      logger.LogInformation($" File {item}");

    foreach (var item in Scripts)
      logger.LogInformation($" Script {item}");

    foreach (var item in Themes)
      logger.LogInformation($" Theme {item}");

    foreach (var item in CounterActions)
      logger.LogInformation($" CounterAction {item}");
  }

}
