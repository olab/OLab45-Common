using Newtonsoft.Json;
using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Utils;

namespace OLab.Api.ObjectMapper
{
  public class ThemesFull : OLabMapper<SystemThemes, ThemesFullDto>
  {
    protected readonly bool enableWikiTranslation;

    public ThemesFull(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
      this.enableWikiTranslation = enableWikiTranslation;
    }

    public override ThemesFullDto PhysicalToDto(SystemThemes phys, ThemesFullDto dto)
    {
      if (enableWikiTranslation)
      {
        dto.HeaderText = _tagProvider.Translate(phys.HeaderText);
        dto.FooterText = _tagProvider.Translate(phys.FooterText);
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
}