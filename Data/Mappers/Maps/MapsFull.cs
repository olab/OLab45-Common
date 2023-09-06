using AutoMapper;
using OLab.Common;
using OLab.Dto;
using OLab.Utils;

namespace OLab.ObjectMapper
{
  public class MapsFullMapper : OLabMapper<Model.Maps, MapsFullDto>
  {
    public MapsFullMapper(OLabLogger logger, bool enableWikiTranslation = true) : base(logger)
    {
    }

    /// <summary>
    /// Default (overridable) AutoMapper configuration
    /// </summary>
    /// <returns>MapperConfiguration</returns>
    protected override MapperConfiguration GetConfiguration()
    {
      return new MapperConfiguration(cfg =>
       cfg.CreateMap<Model.Maps, Dto.MapsFullDto>()
        .ForMember(dest => dest.Description, act => act.MapFrom(src => src.Abstract))
        .ReverseMap()
      );
    }

    public MapsFullMapper(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }

  }
}
