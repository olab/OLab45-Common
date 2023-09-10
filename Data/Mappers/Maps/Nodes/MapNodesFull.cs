using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Utils;

namespace OLab.Api.ObjectMapper
{
  public class MapNodesFullMapper : OLabMapper<Model.MapNodes, MapNodesFullDto>
  {
    protected readonly bool enableWikiTranslation = false;

    public MapNodesFullMapper(
      OLabLogger logger,
      bool enableWikiTranslation = true) : base(logger)
    {
      this.enableWikiTranslation = enableWikiTranslation;
    }

    public MapNodesFullMapper(
      OLabLogger logger,
      WikiTagProvider tagProvider,
      bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
      this.enableWikiTranslation = enableWikiTranslation;
    }

    public override MapNodes DtoToPhysical(MapNodesFullDto dto, MapNodes source)
    {
      source.Rgb = dto.Color;
      return base.DtoToPhysical(dto, source);
    }

    public override MapNodesFullDto PhysicalToDto(MapNodes phys, MapNodesFullDto dto)
    {
      dto.Height = phys.Height.HasValue ? phys.Height : MapNodesMapper.DefaultHeight;

      if (enableWikiTranslation)
        dto.Text = GetWikiProvider().Translate(phys.Text);
      else
        dto.Text = phys.Text;
      dto.Width = phys.Width.HasValue ? phys.Width : MapNodesMapper.DefaultWidth;

      dto.Width = phys.Width.HasValue ? phys.Width : MapNodesMapper.DefaultWidth;
      dto.Color = phys.Rgb;

      if (string.IsNullOrEmpty(dto.Color))
        dto.Color = "#F78749";

      return dto;
    }
  }
}
