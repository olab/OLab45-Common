using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("AV")]
public class AvatarWikiTag : WikiTag1Argument
{
  public AvatarWikiTag(IOLabLogger logger) : base(logger, "OlabAvatarTag")
  {
  }

}