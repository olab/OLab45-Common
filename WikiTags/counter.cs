using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Common.Attributes;

[OLabModule("CR")]
public class CounterWikiTag : WikiTag1Argument
{
  public CounterWikiTag(OLabLogger logger) : base(logger, "OlabCounterTag")
  {
  }
}