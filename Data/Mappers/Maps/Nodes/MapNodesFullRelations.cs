using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Common.Interfaces;
using System.Linq;
using OLab.Api.WikiTag;
using OLab.Api.Model;

namespace OLab.Api.ObjectMapper;

public class MapsNodesFullRelationsMapper : OLabMapper<Model.MapNodes, MapsNodesFullRelationsDto>
{
  protected readonly bool enableWikiTranslation = false;

  public MapsNodesFullRelationsMapper(
    IOLabLogger logger,
    OLabDBContext dbContext,
    WikiTagModuleProvider tagProvider,
    bool enableWikiTranslation = true) : base(logger, dbContext, tagProvider)
  {
    this.enableWikiTranslation = enableWikiTranslation;
  }

  public override MapsNodesFullRelationsDto PhysicalToDto(Model.MapNodes phys, MapsNodesFullRelationsDto dto)
  {
    dto.Height = phys.Height.HasValue ? phys.Height : MapNodesMapper.DefaultHeight;

    if (enableWikiTranslation)
    {
      dto.Text = GetWikiProvider().Translate(phys.Text);
    }
    else
      dto.Text = phys.Text;

    dto.Width = phys.Width.HasValue ? phys.Width : MapNodesMapper.DefaultWidth;
    dto.MapNodeLinks = new MapNodeLinksMapper(
      _logger,
      GetDbContext(),
      GetWikiProvider()).PhysicalToDto(phys.MapNodeLinksNodeId1Navigation.ToList());

    return dto;
  }
}
