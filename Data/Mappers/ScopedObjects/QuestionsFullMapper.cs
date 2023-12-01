using AutoMapper;
using NuGet.Packaging;
using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using System.Linq;

namespace OLab.Api.ObjectMapper
{
  public class QuestionsFullMapper : OLabMapper<SystemQuestions, QuestionsFullDto>
  {
    public QuestionsFullMapper(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
    {
    }

    public QuestionsFullMapper(IOLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
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
      var builder = new QuestionResponses(Logger, GetWikiProvider(), dto);
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

      var builder = new QuestionResponses(Logger, GetWikiProvider(), dto);
      dto.Responses.AddRange(builder.PhysicalToDto(phys.SystemQuestionResponses.ToList()));

      return dto;
    }
  }
}