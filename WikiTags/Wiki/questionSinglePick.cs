using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;
using OLab.Common.Utils;

[OLabModule("QUSP")]
public class QuestionSinglePickWikiTag : QuestionWikiTag
{
  public QuestionSinglePickWikiTag(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base(logger, configuration)
  {
    SetHtmlElementName("OlabSinglePickQuestion");
  }

}