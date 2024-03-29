using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;

namespace OLab.Api.ObjectMapper;

public class MapsFullMapper : OLabMapper<Model.Maps, MapsFullDto>
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
     cfg.CreateMap<Model.Maps, Dto.MapsFullDto>()
      .ForMember(dest => dest.Description, act => act.MapFrom(src => src.Abstract))
      .ReverseMap()
    );
  }

  public override MapsFullDto PhysicalToDto(Maps phys)
  {
    var dto = base.PhysicalToDto(phys);
    dto.Feedback = Conversions.Base64Decode(phys.Feedback);
    return dto;
  }

}
