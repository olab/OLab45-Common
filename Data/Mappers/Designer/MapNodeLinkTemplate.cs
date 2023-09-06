using OLab.Common;
using OLab.Dto.Designer;
using OLab.Utils;

namespace OLab.ObjectMapper
{
  public class MapNodeLinkTemplate : OLabMapper<Model.MapNodeLinks, MapNodeLinkTemplateDto>
  {
    protected readonly bool enableWikiTranslation = false;

    public MapNodeLinkTemplate(OLabLogger logger, bool enableWikiTranslation = true) : base(logger)
    {
      this.enableWikiTranslation = enableWikiTranslation;
    }

    public MapNodeLinkTemplate(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
      this.enableWikiTranslation = enableWikiTranslation;
    }
  }
}
