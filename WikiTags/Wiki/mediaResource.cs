using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule( "MR" )]
public class MediaResourceWikiTag : WikiTag1ArgumentModule
{
  public MediaResourceWikiTag(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base( logger, configuration )
  {
    SetHtmlElementName( "OlabMediaResourceTag" );
  }
}