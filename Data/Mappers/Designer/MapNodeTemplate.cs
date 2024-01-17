using OLab.Api.Common;
using OLab.Api.Models;
using OLab.Common.Interfaces;
using OLab.Data.Dtos.Designer;

namespace OLab.Data.Mappers.Designer;

public class MapNodeTemplate : OLabMapper<MapNodes, MapNodeTemplateDto>
{
  protected readonly bool enableWikiTranslation = false;

  public MapNodeTemplate(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
  {
    this.enableWikiTranslation = enableWikiTranslation;
  }

  public MapNodeTemplate(IOLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
  {
    this.enableWikiTranslation = enableWikiTranslation;
  }
}
