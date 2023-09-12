using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("INFO")]
public class InfoWikiTag : WikiTag1Argument
{
  public InfoWikiTag(IOLabLogger logger) : base(logger, "OlabInfoTag")
  {
  }
}