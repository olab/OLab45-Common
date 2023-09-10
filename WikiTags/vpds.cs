using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Common.Attributes;

[OLabModule("VPD")]
public class VpdWikiTag : WikiTag1Argument
{
  public VpdWikiTag(OLabLogger logger) : base(logger, "")
  {
  }
}