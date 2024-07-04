using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("CR")]
public class CounterWikiTag : WikiTag1ArgumentModule
{
  public CounterWikiTag(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base(logger, configuration)
  {
    SetHtmlElementName("OlabCounterTag");
  }
}