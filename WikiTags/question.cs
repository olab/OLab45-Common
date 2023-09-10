using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Common.Attributes;

[OLabModule("QU")]
public class QuestionWikiTag : WikiTag1Argument
{
  public QuestionWikiTag(OLabLogger logger) : base(logger, "OlabQuestionTag")
  {
  }

}