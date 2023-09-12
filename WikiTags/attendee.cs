using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("ATTENDEE")]
public class AttendeeWikiTag : WikiTag1Argument
{
  public AttendeeWikiTag(IOLabLogger logger) : base(logger, "OlabAttendeeTag")
  {
  }
}