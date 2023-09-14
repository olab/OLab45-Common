using Microsoft.Extensions.Configuration;
using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("JUMP")]
public class JumpWikiTag : WikiTag1Argument
{
  public JumpWikiTag(
    IOLabLogger logger, 
    IConfiguration configuration) : base(logger, configuration, "OlabJumpTag")
  {
  }
}