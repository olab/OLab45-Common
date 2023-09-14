using Microsoft.Extensions.Configuration;
using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("COUNTERS")]
public class CountersWikiTag : WikiTag0Argument
{
  public CountersWikiTag(
    IOLabLogger logger, 
    IConfiguration configuration) : base(logger, configuration, "OlabCountersTag")
  {
  }
}