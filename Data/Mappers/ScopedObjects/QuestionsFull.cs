using AutoMapper;
using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using System.Linq;

namespace OLab.Api.ObjectMapper
{
  public class QuestionsFull : OLabMapper<SystemQuestions, QuestionsFullDto>
  {
    public QuestionsFull(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
    {
    }

    public QuestionsFull(IOLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }

    /// <summary>
    /// Default (overridable) AutoMapper configuration
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

    public override QuestionsFullDto PhysicalToDto(SystemQuestions phys, QuestionsFullDto dto)
    {
      if (string.IsNullOrEmpty(phys.Name))
        dto.Name = phys.Id.ToString();

      // calculated properties
      dto.Wiki = phys.GetWikiTag();
      dto.Value = null;

      var builder = new QuestionResponses(Logger, GetWikiProvider(), dto);
      dto.Responses.AddRange(builder.PhysicalToDto(phys.SystemQuestionResponses.ToList()));

      return dto;
    }
  }
}