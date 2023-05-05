using OLabWebAPI.Common;
using OLabWebAPI.Dto.Designer;
using OLabWebAPI.Utils;

namespace OLabWebAPI.ObjectMapper
{
  public class MapNodeTemplate : OLabMapper<Model.MapNodes, MapNodeTemplateDto>
  {
    protected readonly bool enableWikiTranslation = false;

    public MapNodeTemplate(OLabLogger logger, bool enableWikiTranslation = true) : base(logger)
    {
      this.enableWikiTranslation = enableWikiTranslation;
    }

    public MapNodeTemplate(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
      this.enableWikiTranslation = enableWikiTranslation;
    }
  }
}
