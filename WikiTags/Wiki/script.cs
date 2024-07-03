using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("SCRIPT")]
public class ScriptWikiTag : WikiTag1Argument
{
  public ScriptWikiTag(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base(logger, configuration, "OlabScriptTag")
  {
  }
}