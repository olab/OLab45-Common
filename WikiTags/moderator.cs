using OLab.Common;
using OLab.Utils;

[WikiTagModule("MODERATOR")]
public class ModeratorWikiTag : WikiTag1Argument
{
  public ModeratorWikiTag(OLabLogger logger) : base(logger, "OlabModeratorTag")
  {
  }
}