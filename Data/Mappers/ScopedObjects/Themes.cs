using OLab.Common;
using OLab.Dto;
using OLab.Model;
using OLab.Utils;

namespace OLab.ObjectMapper
{
  public class Themes : OLabMapper<SystemThemes, ThemesDto>
  {
    public Themes(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }
  }
}