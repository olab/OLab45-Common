using OLab.Common;
using OLab.Utils;

[WikiTagModule("ATTENDEE")]
public class AttendeeWikiTag : WikiTag1Argument
{
  public AttendeeWikiTag(OLabLogger logger) : base(logger, "OlabAttendeeTag")
  {
  }
}