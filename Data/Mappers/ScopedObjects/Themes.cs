using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using OLab.Api.WikiTag;

namespace OLab.Api.ObjectMapper;

public class Themes : OLabMapper<SystemThemes, ThemesDto>
{
  public Themes(
    IOLabLogger logger, 
    OLabDBContext dbContext,
    WikiTagModuleProvider tagProvider,
    bool enableWikiTranslation = true) : base(logger, dbContext, tagProvider)
  {
  }
}