using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;
using OLab.Common.Utils;

[OLabModule("QUSD")]
public class QuestionSliderWikiTag : QuestionWikiTag
{
  public QuestionSliderWikiTag(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base(logger, configuration)
  {
    SetHtmlElementName("OlabSliderQuestion");
  }

}