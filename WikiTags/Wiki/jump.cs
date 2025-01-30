using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule( "JUMP" )]
public class JumpWikiTag : WikiTag1ArgumentModule
{
  public JumpWikiTag(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base( logger, configuration )
  {
    SetHtmlElementName( "OlabJumpTag" );
  }
}