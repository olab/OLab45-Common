using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Common.Attributes;

[OLabModule("CONST")]
public class ConstantWikiTag : WikiTag1Argument
{
  public ConstantWikiTag(OLabLogger logger) : base(logger, "OlabConstantTag")
  {
  }
}