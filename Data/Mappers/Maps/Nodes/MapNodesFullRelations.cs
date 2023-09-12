using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Utils;
using System.Linq;
using OLab.Common.Interfaces;

namespace OLab.Api.ObjectMapper
{
    public class MapsNodesFullRelationsMapper : OLabMapper<Model.MapNodes, MapsNodesFullRelationsDto>
  {
    protected readonly bool enableWikiTranslation = false;

    public MapsNodesFullRelationsMapper(
      IOLabLogger logger,
      bool enableWikiTranslation = true) : base(logger)
    {
      this.enableWikiTranslation = enableWikiTranslation;
    }

    public MapsNodesFullRelationsMapper(
      IOLabLogger logger,
      WikiTagProvider tagProvider,
      bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
      this.enableWikiTranslation = enableWikiTranslation;
    }

    public override MapsNodesFullRelationsDto PhysicalToDto(Model.MapNodes phys, MapsNodesFullRelationsDto dto)
    {
      dto.Height = phys.Height.HasValue ? phys.Height : MapNodesMapper.DefaultHeight;

      if (enableWikiTranslation)
        dto.Text = GetWikiProvider().Translate(phys.Text);
      else
        dto.Text = phys.Text;

      dto.Width = phys.Width.HasValue ? phys.Width : ObjectMapper.MapNodesMapper.DefaultWidth;
      dto.MapNodeLinks = new MapNodeLinksMapper(
        Logger,
        GetWikiProvider()).PhysicalToDto(phys.MapNodeLinksNodeId1Navigation.ToList());

      return dto;
    }
  }
}
