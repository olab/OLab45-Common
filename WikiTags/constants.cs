using OLab.Api.Common;
using OLab.Api.Utils;

[WikiTagModule("CONST")]
public class ConstantWikiTag : WikiTag1Argument
{
  public static ConstantWikiTag Instance()
  {
    return new ConstantWikiTag(new OLabLogger());
  }

  public ConstantWikiTag(OLabLogger logger) : base(logger, "OlabConstantTag")
  {
  }
}