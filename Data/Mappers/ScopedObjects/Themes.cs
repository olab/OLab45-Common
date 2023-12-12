using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Common.Interfaces;
using OLab.Data.BusinessObjects;

namespace OLab.Api.ObjectMapper
{
    public class Themes : OLabMapper<SystemThemes, ThemesDto>
  {
    public Themes(IOLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }
  }
}