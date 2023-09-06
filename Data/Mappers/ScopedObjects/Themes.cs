using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Utils;

namespace OLab.Api.ObjectMapper
{
  public class Themes : OLabMapper<SystemThemes, ThemesDto>
  {
    public Themes(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }
  }
}