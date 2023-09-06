using OLab.Api.Common;
using OLab.Api.Utils;

[WikiTagModule("MODERATOR")]
public class ModeratorWikiTag : WikiTag1Argument
{
  public ModeratorWikiTag(OLabLogger logger) : base(logger, "OlabModeratorTag")
  {
  }
}