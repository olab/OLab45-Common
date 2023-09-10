using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Common.Attributes;

[OLabModule("REPORT")]
public class ReportWikiTag : WikiTag0Argument
{
  public ReportWikiTag(OLabLogger logger) : base(logger, "OlabReportTag")
  {
  }
}