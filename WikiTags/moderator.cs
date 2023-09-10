using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Common.Attributes;

[OLabModule("MODERATOR")]
public class ModeratorWikiTag : WikiTag1Argument
{
  public ModeratorWikiTag(OLabLogger logger) : base(logger, "OlabModeratorTag")
  {
  }
}