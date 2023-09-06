using OLab.Common;
using OLab.Dto;
using OLab.Model;
using OLab.Utils;

namespace OLab.ObjectMapper
{
  public class MapNodesFullMapper : OLabMapper<Model.MapNodes, MapNodesFullDto>
  {
    protected readonly bool enableWikiTranslation = false;

    public MapNodesFullMapper(OLabLogger logger, bool enableWikiTranslation = true) : base(logger)
    {
      this.enableWikiTranslation = enableWikiTranslation;
    }

    public MapNodesFullMapper(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
      this.enableWikiTranslation = enableWikiTranslation;
    }

    public override MapNodes DtoToPhysical(MapNodesFullDto dto, MapNodes source)
    {
      source.Rgb = dto.Color;
      return base.DtoToPhysical(dto, source);
    }

    public override MapNodesFullDto PhysicalToDto(Model.MapNodes phys, MapNodesFullDto dto)
    {
      dto.Height = phys.Height.HasValue ? phys.Height : ObjectMapper.MapNodesMapper.DefaultHeight;

      if (enableWikiTranslation)
        dto.Text = _tagProvider.Translate(phys.Text);
      else
        dto.Text = phys.Text;
      dto.Width = phys.Width.HasValue ? phys.Width : ObjectMapper.MapNodesMapper.DefaultWidth;

      dto.Width = phys.Width.HasValue ? phys.Width : ObjectMapper.MapNodesMapper.DefaultWidth;
      dto.Color = phys.Rgb;

      if (string.IsNullOrEmpty(dto.Color))
        dto.Color = "#F78749";

      return dto;
    }
  }
}
