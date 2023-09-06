using OLab.Api.Common;
using OLab.Api.Utils;

[WikiTagModule("ATTENDEE")]
public class AttendeeWikiTag : WikiTag1Argument
{
  public AttendeeWikiTag(OLabLogger logger) : base(logger, "OlabAttendeeTag")
  {
  }
}