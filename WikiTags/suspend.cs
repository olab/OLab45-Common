using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("SUSPEND")]
public class SuspendWikiTag : WikiTag0Argument
{
  public SuspendWikiTag(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base(logger, configuration, "OlabSuspendTag")
  {
  }
}