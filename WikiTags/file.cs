using OLab.Api.Common;
using OLab.Api.Utils;

[WikiTagModule("FILE")]
public class FileWikiTag : WikiTag1Argument
{
  public FileWikiTag(OLabLogger logger) : base(logger, "OlabFileTag")
  {
  }
}