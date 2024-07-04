using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;
using OLab.Common.Utils;

[OLabModule("QUDG")]
public class QuestionDragDropWikiTag : QuestionWikiTag
{
  public QuestionDragDropWikiTag(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base(logger, configuration)
  {
    SetHtmlElementName("OlabDragAndDropQuestion");
  }

}