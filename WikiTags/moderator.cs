using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("MODERATOR")]
public class ModeratorWikiTag : WikiTag1Argument
{
  public ModeratorWikiTag(IOLabLogger logger) : base(logger, "OlabModeratorTag")
  {
  }
}