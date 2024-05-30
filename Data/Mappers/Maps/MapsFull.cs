using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Common.Utils;
using System;

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
    {
      cfg.CreateMap<Maps, MapsFullDto>()
        .ForMember(dest => dest.Description, act => act.MapFrom(src => src.Abstract))
        .ReverseMap();
      cfg.CreateMap<MapGroups, MapGroupsDto>().ReverseMap();
    });

  }

  public override MapsFullDto PhysicalToDto(Maps mapPhys)
  {
    var dto = base.PhysicalToDto(mapPhys);

    dto.Feedback = Conversions.Base64Decode(mapPhys.Feedback);
    dto.CreatedAt = TimeUtils.ToUtc(mapPhys.CreatedAt);

    return dto;
  }

  public override Maps DtoToPhysical(MapsFullDto dto)
  {
    var mapPhys = base.DtoToPhysical(dto);

    mapPhys.CreatedAt = TimeUtils.ToUtc(dto.CreatedAt);
    return mapPhys;
  }
}
