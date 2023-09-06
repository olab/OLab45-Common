using OLab.Api.Common;
using OLab.Api.Utils;

[WikiTagModule("CR")]
public class CounterWikiTag : WikiTag1Argument
{
  public CounterWikiTag(OLabLogger logger) : base(logger, "OlabCounterTag")
  {
  }
}