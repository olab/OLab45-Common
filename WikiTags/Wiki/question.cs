using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("QU")]
public class QuestionWikiTag : WikiTag1ArgumentModule
{
  public QuestionWikiTag(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base(logger, configuration, "OlabQuestionTag")
  {
  }

}