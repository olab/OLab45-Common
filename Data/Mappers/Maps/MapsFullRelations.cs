using AutoMapper;
using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace OLab.Api.ObjectMapper;

public class MapsFullRelationsMapper : OLabMapper<Maps, MapsFullRelationsDto>
{
  private readonly bool _enableWikiTranslation;

  public MapsFullRelationsMapper(
    IOLabLogger logger,
    bool enableWikiTranslation = true) : base(logger)
  {
    _enableWikiTranslation = enableWikiTranslation;
  }

  public MapsFullRelationsMapper(
    IOLabLogger logger,
    WikiTagProvider tagProvider,
    bool enableWikiTranslation = true) : base(logger, tagProvider)
  {
    _enableWikiTranslation = enableWikiTranslation;
  }

  /// <summary>
  /// Default (overridable) AutoMapper cfg
  /// </summary>
  /// <returns>MapperConfiguration</returns>
  protected override MapperConfiguration GetConfiguration()
  {
    return new MapperConfiguration(cfg =>
    {
      cfg.CreateMap<Maps, MapsFullRelationsDto>().ReverseMap();
      cfg.CreateMap<MapNodes, MapNodesFullDto>().ReverseMap();
      cfg.CreateMap<MapNodeLinks, MapNodeLinksFullDto>().ReverseMap();
    });
  }

  /// <summary>
  /// Convert a physical object to new dto. 
  /// </summary>
  /// <remarks>
  /// Allows for derived class specific overrides that 
  /// don't fit well with default implementation
  /// </remarks>
  /// <param name="phys">Physical object</param>
  /// <returns>Dto object</returns>
  public override MapsFullRelationsDto PhysicalToDto(Maps map)
  {
    var dto = new MapsFullRelationsDto
    {
      Map = new MapsFullMapper(
        Logger,
        _wikiTagModules,
        _enableWikiTranslation
      ).PhysicalToDto(map),
      MapNodes = new MapNodesFullMapper(
        Logger,
        _wikiTagModules,
        _enableWikiTranslation
      ).PhysicalToDto(map.MapNodes.ToList())
    };

    var links = new List<MapNodeLinks>();
    links.AddRange(map.MapNodeLinks);

    //foreach (var node in map.MapNodes)
    //  links.AddRange(node.MapNodeLinksNodeId1Navigation);

    dto.MapNodeLinks = new MapNodeLinksMapper(Logger, _enableWikiTranslation).PhysicalToDto(links);

    return dto;
  }

}
