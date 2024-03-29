using OLab.Api.Common;
using OLab.Api.Dto.Designer;
using OLab.Common.Interfaces;

namespace OLab.Api.ObjectMapper;

public class MapNodeLinkTemplate : OLabMapper<Model.MapNodeLinks, MapNodeLinkTemplateDto>
{
  protected readonly bool enableWikiTranslation = false;

  public MapNodeLinkTemplate(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
  {
    this.enableWikiTranslation = enableWikiTranslation;
  }

  public MapNodeLinkTemplate(IOLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
  {
    this.enableWikiTranslation = enableWikiTranslation;
  }
}
