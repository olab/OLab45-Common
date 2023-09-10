using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Common.Attributes;

[OLabModule("LINKS")]
public class LinksWikiTag : WikiTag0Argument
{
  public LinksWikiTag(OLabLogger logger) : base(logger, "OlabLinksTag")
  {
  }
}