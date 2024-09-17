using OLab.Common.Attributes;
using OLab.Common.Interfaces;

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