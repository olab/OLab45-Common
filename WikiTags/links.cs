using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("LINKS")]
public class LinksWikiTag : WikiTag0Argument
{
  public LinksWikiTag(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base(logger, configuration, "OlabLinksTag")
  {
  }
}