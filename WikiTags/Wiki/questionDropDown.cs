using OLab.Common.Attributes;
using OLab.Common.Interfaces;

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