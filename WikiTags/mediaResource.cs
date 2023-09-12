using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("MR")]
public class MediaResourceWikiTag : WikiTag1Argument
{
  public MediaResourceWikiTag(IOLabLogger logger) : base(logger, "OlabMediaResourceTag")
  {
  }
}