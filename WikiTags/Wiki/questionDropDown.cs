using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;
using OLab.Common.Utils;

[OLabModule("QUDP")]
public class QuestionDropDownWikiTag : QuestionWikiTag
{
  public QuestionDropDownWikiTag(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base(logger, configuration)
  {
    SetHtmlElementName("OlabDropDownQuestion");
  }

}