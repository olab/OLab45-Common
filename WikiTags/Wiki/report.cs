using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("REPORT")]
public class ReportWikiTag : WikiTag0ArgumentModule
{
  public ReportWikiTag(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base(logger, configuration)
  {
    SetHtmlElementName("OlabReportTag");
  }
}