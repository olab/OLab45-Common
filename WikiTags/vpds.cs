using Microsoft.Extensions.Configuration;
using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("VPD")]
public class VpdWikiTag : WikiTag1Argument
{
  public VpdWikiTag(
    IOLabLogger logger, 
    IConfiguration configuration) : base(logger, configuration, "")
  {
  }
}