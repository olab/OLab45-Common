using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("CONST")]
public class ConstantWikiTag : WikiTag1Argument
{
  public ConstantWikiTag(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base(logger, configuration, "OlabConstantTag")
  {
  }
}