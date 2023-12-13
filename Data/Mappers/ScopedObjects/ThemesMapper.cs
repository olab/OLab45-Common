using OLab.Api.Common;
using OLab.Common.Interfaces;
using OLab.Data.Dtos;
using OLab.Data.Models;

namespace OLab.Data.Mappers
{
  public class ThemesMapper : OLabMapper<SystemThemes, ThemesDto>
  {
    public ThemesMapper(IOLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }
  }
}