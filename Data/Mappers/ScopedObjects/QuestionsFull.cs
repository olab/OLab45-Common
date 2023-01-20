using AutoMapper;
using OLabWebAPI.Common;
using OLabWebAPI.Dto;
using OLabWebAPI.Model;
using OLabWebAPI.Utils;
using System.Linq;

namespace OLabWebAPI.ObjectMapper
{
    public class QuestionsFull : OLabMapper<SystemQuestions, QuestionsFullDto>
    {
        public QuestionsFull(OLabLogger logger, bool enableWikiTranslation = true) : base(logger)
        {
        }

        public QuestionsFull(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
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

            var builder = new ObjectMapper.QuestionResponses(logger, GetWikiProvider(), dto);
            dto.Responses.AddRange(builder.PhysicalToDto(phys.SystemQuestionResponses.ToList()));

            return dto;
        }
    }
}