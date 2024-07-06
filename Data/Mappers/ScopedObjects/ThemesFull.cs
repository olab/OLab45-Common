using Newtonsoft.Json;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.WikiTag;
using OLab.Common.Interfaces;
using OLab.Data.ReaderWriters;

namespace OLab.Api.ObjectMapper;

public class ThemesFull : OLabMapper<SystemThemes, ThemesFullDto>
{
  protected readonly bool enableWikiTranslation;

  public ThemesFull(
    IOLabLogger logger,
    OLabDBContext dbContext,
    WikiTagModuleProvider tagProvider = null,
    bool enableWikiTranslation = true) : base(logger, dbContext, tagProvider)
  {
    this.enableWikiTranslation = enableWikiTranslation;
  }

  public override ThemesFullDto PhysicalToDto(SystemThemes phys, ThemesFullDto dto)
  {
    if (enableWikiTranslation)
    {
      dto.HeaderText = GetWikiProvider().Translate(phys.HeaderText);
      dto.FooterText = GetWikiProvider().Translate(phys.FooterText);
    }
    else
    {
      dto.HeaderText = phys.HeaderText;
      dto.FooterText = phys.FooterText;
    }
    return dto;
  }

  [JsonProperty("header")]
  public string HeaderText { get; set; }
  [JsonProperty("footer")]
  public string FooterText { get; set; }
}