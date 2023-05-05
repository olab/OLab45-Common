using OLabWebAPI.Common;
using OLabWebAPI.Dto;
using OLabWebAPI.Model;
using OLabWebAPI.Utils;

namespace OLabWebAPI.ObjectMapper
{
  public class Themes : OLabMapper<SystemThemes, ThemesDto>
  {
    public Themes(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }
  }
}