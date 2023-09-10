using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Common.Attributes;

[OLabModule("INFO")]
public class InfoWikiTag : WikiTag1Argument
{
  public InfoWikiTag(OLabLogger logger) : base(logger, "OlabInfoTag")
  {
  }
}