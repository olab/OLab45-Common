using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;
using OLab.Common.Utils;

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