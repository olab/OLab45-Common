using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Common.Interfaces;
using System.Linq;
using OLab.Api.WikiTag;
using OLab.Api.Model;
using OLab.Data.ReaderWriters;
using NuGet.Packaging;

namespace OLab.Api.ObjectMapper;

public class MapsNodesFullRelationsMapper : OLabMapper<Model.MapNodes, MapsNodesFullRelationsDto>
{
  protected readonly bool enableWikiTranslation = false;
  private readonly QuestionReaderWriter _reader;

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
  }

  public override MapsNodesFullRelationsDto PhysicalToDto(Model.MapNodes phys, MapsNodesFullRelationsDto dto)
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
    dto.MapNodeLinks.AddRange( new MapNodeLinksMapper(
      _logger,
      GetDbContext(),
      GetWikiProvider()).PhysicalToDto(phys.MapNodeLinksNodeId1Navigation.ToList()));

    dto.MapNodeGroupRoles.AddRange(new MapNodeGroupRolesMapper(
      _logger,
      GetDbContext(),
      GetWikiProvider()).PhysicalToDto(phys.MapNodeGrouproles.ToList()));

    return dto;
  }
}
