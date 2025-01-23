using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule( "AV" )]
public class AvatarWikiTag : WikiTag1ArgumentModule
{
  public AvatarWikiTag(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base( logger, configuration )
  {
    SetHtmlElementName( "OlabAvatarTag" );
  }

}