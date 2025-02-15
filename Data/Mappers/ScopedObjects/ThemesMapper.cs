using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.WikiTag;
using OLab.Common.Interfaces;

namespace OLab.Api.ObjectMapper;

public class ThemesMapper : OLabMapper<SystemThemes, ThemesDto>
{
  public ThemesMapper(
    IOLabLogger logger,
    OLabDBContext dbContext,
    WikiTagModuleProvider tagProvider,
    bool enableWikiTranslation = true) : base( logger, dbContext, tagProvider )
  {
  }
}