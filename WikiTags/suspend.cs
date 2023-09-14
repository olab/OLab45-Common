using Microsoft.Extensions.Configuration;
using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("SUSPEND")]
public class SuspendWikiTag : WikiTag0Argument
{
  public SuspendWikiTag(
    IOLabLogger logger, 
    IConfiguration configuration) : base(logger, configuration, "OlabSuspendTag")
  {
  }
}