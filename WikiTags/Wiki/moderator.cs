using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("MODERATOR")]
public class ModeratorWikiTag : WikiTag1ArgumentModule
{
  public ModeratorWikiTag(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base(logger, configuration, "OlabModeratorTag")
  {
  }
}