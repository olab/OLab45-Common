using OLab.Common;
using OLab.Utils;

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