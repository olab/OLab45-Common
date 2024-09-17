using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("QUMP")]
public class QuestionMultiPickWikiTag : QuestionWikiTag
{
  public QuestionMultiPickWikiTag(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base(logger, configuration)
  {
    SetHtmlElementName("OlabMultiPickQuestion");
  }

}