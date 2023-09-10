using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Common.Attributes;

[OLabModule("ATTENDEE")]
public class AttendeeWikiTag : WikiTag1Argument
{
  public AttendeeWikiTag(OLabLogger logger) : base(logger, "OlabAttendeeTag")
  {
  }
}