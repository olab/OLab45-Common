using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("REPORT")]
public class ReportWikiTag : WikiTag0Argument
{
  public ReportWikiTag(IOLabLogger logger) : base(logger, "OlabReportTag")
  {
  }
}