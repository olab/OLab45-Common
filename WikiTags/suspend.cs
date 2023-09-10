using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Common.Attributes;

[OLabModule("SUSPEND")]
public class SuspendWikiTag : WikiTag0Argument
{
  public SuspendWikiTag(OLabLogger logger) : base(logger, "OlabSuspendTag")
  {
  }
}