using OLab.Api.Common;
using OLab.Api.Utils;

[WikiTagModule("MR")]
public class MediaResourceWikiTag : WikiTag1Argument
{
  public MediaResourceWikiTag(OLabLogger logger) : base(logger, "OlabMediaResourceTag")
  {
  }
}