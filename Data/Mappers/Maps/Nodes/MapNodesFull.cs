using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.WikiTag;
using OLab.Common.Interfaces;
using OLab.Data.ReaderWriters;

namespace OLab.Api.ObjectMapper;

public class MapNodesFullMapper : OLabMapper<MapNodes, MapNodesFullDto>
{
  protected readonly bool enableWikiTranslation = false;
  private readonly QuestionReaderWriter _reader;

  public MapNodesFullMapper(
    IOLabLogger logger,
    OLabDBContext dbContext,
    IOLabModuleProvider<IWikiTagModule> tagProvider,
    bool enableWikiTranslation = true) : base(logger, dbContext, tagProvider)
  {
    this.enableWikiTranslation = enableWikiTranslation;

    _reader = new QuestionReaderWriter(
      GetLogger(),
      GetDbContext(),
      tagProvider);
  }

  public override MapNodes DtoToPhysical(MapNodesFullDto dto, MapNodes source)
  {
    source.Rgb = dto.Color;
    return base.DtoToPhysical(dto, source);
  }

  public override MapNodesFullDto PhysicalToDto(MapNodes phys, MapNodesFullDto dto)
  {
    dto.Height = phys.Height.HasValue ? phys.Height : MapNodesMapper.DefaultHeight;
    dto.Text = phys.Text;

    if (enableWikiTranslation)
    {
      dto.Text = _reader.DisambiguateWikiQuestions(dto.Text);
      dto.Text = GetWikiProvider().Translate(dto.Text);
    }
    else

    dto.Width = phys.Width.HasValue ? phys.Width : MapNodesMapper.DefaultWidth;
    dto.Color = phys.Rgb;

    if (string.IsNullOrEmpty(dto.Color))
      dto.Color = "#F78749";

    return dto;
  }
}
