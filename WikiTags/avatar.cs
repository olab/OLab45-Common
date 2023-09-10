using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Common.Attributes;

[OLabModule("AV")]
public class AvatarWikiTag : WikiTag1Argument
{
  public AvatarWikiTag(OLabLogger logger) : base(logger, "OlabAvatarTag")
  {
  }

}