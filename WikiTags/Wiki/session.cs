using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("SESSION")]
public class SessionWikiTag : WikiTag0Argument
{
  public SessionWikiTag(IOLabLogger logger,
    IOLabConfiguration configuration) : base(logger, configuration, "OlabSessionTag")
  {
  }
}