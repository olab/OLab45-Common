using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule( "ATTENDEE" )]
public class AttendeeWikiTag : WikiTag1ArgumentModule
{
  public AttendeeWikiTag(IOLabLogger logger, IOLabConfiguration configuration) : base( logger, configuration )
  {
    SetHtmlElementName( "OlabAttendeeTag" );
  }
}