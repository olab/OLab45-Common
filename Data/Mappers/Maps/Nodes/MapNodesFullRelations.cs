using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Common.Interfaces;
using System.Linq;
using OLab.Api.WikiTag;
using OLab.Api.Model;
using OLab.Data.ReaderWriters;
using NuGet.Packaging;
using AutoMapper;

namespace OLab.Api.ObjectMapper;

public class MapsNodesFullRelationsMapper : OLabMapper<MapNodes, MapsNodesFullRelationsDto>
{
  protected readonly bool enableWikiTranslation = false;
  private readonly QuestionReaderWriter _reader;
  private readonly MapNodeGroupRolesMapper _groupRoleMapper;

  public MapsNodesFullRelationsMapper(
    IOLabLogger logger,
    OLabDBContext dbContext,
    WikiTagModuleProvider tagProvider,
    bool enableWikiTranslation = true) : base(logger, dbContext, tagProvider)
  {
    this.enableWikiTranslation = enableWikiTranslation;

    _reader = new QuestionReaderWriter(
      GetLogger(),
      GetDbContext(),
      tagProvider);

    _groupRoleMapper = new MapNodeGroupRolesMapper(GetLogger(), GetDbContext(), tagProvider);

  }

  /// <summary>
  /// Default (overridable) AutoMapper cfg
  /// </summary>
  /// <returns>MapperConfiguration</returns>
  protected override MapperConfiguration GetConfiguration()
  {
    return new MapperConfiguration(cfg =>
    {
      cfg.CreateMap<MapNodes, MapsNodesFullRelationsDto>().ReverseMap();
      cfg.CreateMap<MapNodeLinks, MapNodeLinksFullDto>().ReverseMap();
      cfg.CreateMap<MapNodeGrouproles, MapNodeGroupRolesDto>().ReverseMap();
    });
  }

  public override MapsNodesFullRelationsDto PhysicalToDto(
    MapNodes phys,
    MapsNodesFullRelationsDto dto)
  {
    dto.Height = phys.Height.HasValue ? phys.Height : MapNodesMapper.DefaultHeight;
    dto.Text = phys.Text;

    if (enableWikiTranslation)
    {
      dto.Text = _reader.DisambiguateWikiQuestions(dto.Text);
      dto.Text = GetWikiProvider().Translate(dto.Text);
    }
    else
      dto.Text = phys.Text;

    dto.Width = phys.Width.HasValue ? phys.Width : MapNodesMapper.DefaultWidth;
    dto.MapNodeLinks.AddRange(new MapNodeLinksMapper(
      _logger,
      GetDbContext(),
      GetWikiProvider()).PhysicalToDto(phys.MapNodeLinksNodeId1Navigation.ToList()));

    dto.MapNodeGrouproles.Clear();
    foreach (var groupRolePhys in phys.MapNodeGrouproles)
      dto.MapNodeGrouproles.Add(_groupRoleMapper.PhysicalToDto(groupRolePhys));

    return dto;
  }
}
