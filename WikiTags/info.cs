using OLab.Api.Common;
using OLab.Api.Utils;

[WikiTagModule("INFO")]
public class InfoWikiTag : WikiTag1Argument
{
  public InfoWikiTag(OLabLogger logger) : base(logger, "OlabInfoTag")
  {
  }
}