using OLab.Api.Common;
using OLab.Api.Models;
using OLab.Common.Interfaces;
using OLab.Data.Dtos;
using OLab.Data.Mappers;
using System.Linq;

namespace OLab.Data.Mappers
{
  public class MapsNodesFullRelationsMapper : OLabMapper<MapNodes, MapsNodesFullRelationsDto>
  {
    protected readonly bool enableWikiTranslation = false;

    public MapsNodesFullRelationsMapper(
      IOLabLogger logger,
      WikiTagProvider tagProvider,
      bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
      this.enableWikiTranslation = enableWikiTranslation;
    }

    public override MapsNodesFullRelationsDto PhysicalToDto(MapNodes phys, MapsNodesFullRelationsDto dto)
    {
      dto.Height = phys.Height.HasValue ? phys.Height : MapNodesMapper.DefaultHeight;

      if (enableWikiTranslation)
        dto.Text = GetWikiProvider().Translate(phys.Text);
      else
        dto.Text = phys.Text;

      dto.Width = phys.Width.HasValue ? phys.Width : MapNodesMapper.DefaultWidth;
      dto.MapNodeLinks = new MapNodeLinksMapper(
        Logger,
        GetWikiProvider()).PhysicalToDto(phys.MapNodeLinksNodeId1Navigation.ToList());

      return dto;
    }
  }
}
