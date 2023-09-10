using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Common.Attributes;

[OLabModule("COUNTERS")]
public class CountersWikiTag : WikiTag0Argument
{
  public CountersWikiTag(OLabLogger logger) : base(logger, "OlabCountersTag")
  {
  }
}