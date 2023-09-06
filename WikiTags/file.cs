using OLab.Common;
using OLab.Utils;

[WikiTagModule("FILE")]
public class FileWikiTag : WikiTag1Argument
{
  public FileWikiTag(OLabLogger logger) : base(logger, "OlabFileTag")
  {
  }
}