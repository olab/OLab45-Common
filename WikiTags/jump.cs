using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Common.Attributes;

[OLabModule("JUMP")]
public class JumpWikiTag : WikiTag1Argument
{
  public JumpWikiTag(OLabLogger logger) : base(logger, "OlabJumpTag")
  {
  }
}