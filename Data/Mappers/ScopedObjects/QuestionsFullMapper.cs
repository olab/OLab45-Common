using AutoMapper;
using NuGet.Packaging;
using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using System.Linq;
using OLab.Api.WikiTag;

namespace OLab.Api.ObjectMapper;

public class QuestionsFullMapper : OLabMapper<SystemQuestions, QuestionsFullDto>
{
  public QuestionsFullMapper(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
  {
  }

  public QuestionsFullMapper(IOLabLogger logger, WikiTagModuleProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
  {
  }

  /// <summary>
  /// Default (overridable) AutoMapper cfg
  /// </summary>
  /// <returns>MapperConfiguration</returns>
  protected override MapperConfiguration GetConfiguration()
  {
    return new MapperConfiguration(cfg =>
     cfg.CreateMap<SystemQuestions, QuestionsFullDto>()
      .ForMember(dest => dest.TryCount, act => act.MapFrom(src => src.NumTries))
      .ReverseMap()
    );
  }

  public override SystemQuestions DtoToPhysical(QuestionsFullDto dto)
  {
    var phys = new SystemQuestions();
    _mapper.Map(dto, phys);

    phys = DtoToPhysical(dto, phys);

    return phys;
  }

  public override SystemQuestions DtoToPhysical(QuestionsFullDto dto, SystemQuestions phys)
  {
    var builder = new QuestionResponses(_logger, GetWikiProvider(), dto);
    phys.SystemQuestionResponses.AddRange(builder.DtoToPhysical(dto.Responses));

    return phys;
  }

  public override QuestionsFullDto PhysicalToDto(SystemQuestions phys, QuestionsFullDto dto)
  {
    if (string.IsNullOrEmpty(phys.Name))
      dto.Name = phys.Id.ToString();

    // calculated properties
    dto.Wiki = phys.GetWikiTag();
    dto.Value = null;
    dto.Settings = Conversions.Base64Decode(phys.Settings);

    // catch if empty settings string. replace with default object
    if (string.IsNullOrEmpty(dto.Settings))
    {
      if (phys.EntryTypeId == (uint)SystemQuestionTypes.Type.Slider)
      {
        GetLogger().LogWarning("default slider settings applied");
        dto.Settings = "{\"minValue\":\"1\",\"maxValue\":\"100\",\"stepValue\":\"5\",\"orientation\":\"hor\",\"showValue\":\"0\",\"sliderSkin\":\"\",\"abilityValue\":\"1\",\"defaultValue\":\"50\"}";
      }
    }

    var builder = new QuestionResponses(_logger, GetWikiProvider(), dto);
    dto.Responses.AddRange(builder.PhysicalToDto(phys.SystemQuestionResponses.ToList()));

    return dto;
  }
}