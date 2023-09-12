using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("CR")]
public class CounterWikiTag : WikiTag1Argument
{
  public CounterWikiTag(IOLabLogger logger) : base(logger, "OlabCounterTag")
  {
  }
}