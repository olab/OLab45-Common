using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("COUNTERS")]
public class CountersWikiTag : WikiTag0Argument
{
  public CountersWikiTag(IOLabLogger logger) : base(logger, "OlabCountersTag")
  {
  }
}