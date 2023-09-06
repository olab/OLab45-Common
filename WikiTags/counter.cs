using OLab.Common;
using OLab.Utils;

[WikiTagModule("CR")]
public class CounterWikiTag : WikiTag1Argument
{
  public CounterWikiTag(OLabLogger logger) : base(logger, "OlabCounterTag")
  {
  }
}