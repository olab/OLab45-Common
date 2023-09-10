using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Common.Attributes;

[OLabModule("MR")]
public class MediaResourceWikiTag : WikiTag1Argument
{
  public MediaResourceWikiTag(OLabLogger logger) : base(logger, "OlabMediaResourceTag")
  {
  }
}