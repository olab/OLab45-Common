using OLab.Api.Common;
using OLab.Api.Dto.Designer;
using OLab.Api.Utils;
using OLab.Common.Interfaces;

namespace OLab.Api.ObjectMapper
{
    public class MapNodeTemplate : OLabMapper<Model.MapNodes, MapNodeTemplateDto>
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
}
