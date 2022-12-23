using OLabWebAPI.Common;
using OLabWebAPI.Dto;
using OLabWebAPI.Utils;
using System.Linq;

namespace OLabWebAPI.ObjectMapper
{
  public class MapsNodesFullRelationsMapper : OLabMapper<Model.MapNodes, MapsNodesFullRelationsDto>
  {
    protected readonly bool enableWikiTranslation = false;

    public MapsNodesFullRelationsMapper(OLabLogger logger, bool enableWikiTranslation = true) : base(logger)
    {
      this.enableWikiTranslation = enableWikiTranslation;
    }

    public MapsNodesFullRelationsMapper(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
      this.enableWikiTranslation = enableWikiTranslation;
    }

    public override MapsNodesFullRelationsDto PhysicalToDto(Model.MapNodes phys, MapsNodesFullRelationsDto dto)
    {
      dto.Height = phys.Height.HasValue ? phys.Height : ObjectMapper.MapNodesMapper.DefaultHeight;

      if (enableWikiTranslation)
        dto.Text = _tagProvider.Translate(phys.Text);
      else
        dto.Text = phys.Text;

      dto.Width = phys.Width.HasValue ? phys.Width : ObjectMapper.MapNodesMapper.DefaultWidth;
      dto.MapNodeLinks = new MapNodeLinksMapper(logger, new WikiTagProvider(logger)).PhysicalToDto(phys.MapNodeLinksNodeId1Navigation.ToList());

      return dto;
    }
  }
}
