using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;
using OLab.Common.Utils;

[OLabModule("QUMT")]
public class QuestionMultilineTextWikiTag : QuestionWikiTag
{
  public QuestionMultilineTextWikiTag(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base(logger, configuration)
  {
    SetHtmlElementName("OlabMultilineTextQuestion");
  }

}