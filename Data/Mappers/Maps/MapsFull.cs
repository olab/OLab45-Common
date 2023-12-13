using AutoMapper;
using OLab.Api.Common;
using OLab.Common.Interfaces;
using OLab.Data.Dtos;
using OLab.Data.Models;
using OLab.Data.Mappers;

namespace OLab.Data.Mappers
{
  public class MapsFullMapper : OLabMapper<Api.Models.Maps, MapsFullDto>
  {
    public MapsFullMapper(
      IOLabLogger logger,
      bool enableWikiTranslation = true) : base(logger)
    {
    }
    public MapsFullMapper(
      IOLabLogger logger,
      WikiTagProvider tagProvider,
      bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }

    /// <summary>
    /// Default (overridable) AutoMapper cfg
    /// </summary>
    /// <returns>MapperConfiguration</returns>
    protected override MapperConfiguration GetConfiguration()
    {
      return new MapperConfiguration(cfg =>
       cfg.CreateMap<Api.Models.Maps, MapsFullDto>()
        .ForMember(dest => dest.Description, act => act.MapFrom(src => src.Abstract))
        .ReverseMap()
      );
    }

  }
}
