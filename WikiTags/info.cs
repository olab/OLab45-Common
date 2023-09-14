using Microsoft.Extensions.Configuration;
using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("INFO")]
public class InfoWikiTag : WikiTag1Argument
{
  public InfoWikiTag(
    IOLabLogger logger, 
    IConfiguration configuration) : base(logger, configuration, "OlabInfoTag")
  {
  }
}