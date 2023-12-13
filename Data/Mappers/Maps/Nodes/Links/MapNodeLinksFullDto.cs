using AutoMapper;
using OLab.Api.Common;
using OLab.Api.Models;
using OLab.Common.Interfaces;
using OLab.Data.Dtos;

namespace OLab.Data.Mappers
{
  public class MapNodeLinksFullMapper : OLabMapper<MapNodeLinks, MapNodeLinksFullDto>
  {
    public MapNodeLinksFullMapper(
      IOLabLogger logger,
      bool enableWikiTranslation = true) : base(logger)
    {
    }

    public MapNodeLinksFullMapper(
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
       cfg.CreateMap<MapNodeLinks, MapNodeLinksFullDto>()
        .ForMember(dest => dest.SourceId, act => act.MapFrom(src => src.NodeId1))
        .ForMember(dest => dest.DestinationId, act => act.MapFrom(src => src.NodeId2))
        .ReverseMap()
      );
    }

  }
}
