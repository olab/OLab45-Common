using Microsoft.Extensions.Configuration;
using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("ATTENDEE")]
public class AttendeeWikiTag : WikiTag1Argument
{
  public AttendeeWikiTag(IOLabLogger logger, IConfiguration configuration) : base(logger, configuration, "OlabAttendeeTag")
  {
  }
}