using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("VPD")]
public class VpdWikiTag : WikiTag1Argument
{
  public VpdWikiTag(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base(logger, configuration, "")
  {
  }
}