using Microsoft.Extensions.Configuration;
using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("QU")]
public class QuestionWikiTag : WikiTag1Argument
{
  public QuestionWikiTag(
    IOLabLogger logger, 
    IConfiguration configuration) : base(logger, configuration, "OlabQuestionTag")
  {
  }

}