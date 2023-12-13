using OLab.Api.Common;
using OLab.Api.Models;
using OLab.Common.Interfaces;
using OLab.Data.Dtos;
using OLab.Data.Dtos.Designer;

namespace OLab.Data.Mappers.Designer
{
  public class MapNodeLinkTemplateMapper : OLabMapper<MapNodeLinks, MapNodeLinkTemplateDto>
  {
    protected readonly bool enableWikiTranslation = false;

    public MapNodeLinkTemplateMapper(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
    {
      this.enableWikiTranslation = enableWikiTranslation;
    }

    public MapNodeLinkTemplateMapper(IOLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
      this.enableWikiTranslation = enableWikiTranslation;
    }
  }
}
