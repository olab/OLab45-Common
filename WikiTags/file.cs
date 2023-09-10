using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Common.Attributes;

[OLabModule("FILE")]
public class FileWikiTag : WikiTag1Argument
{
  public FileWikiTag(OLabLogger logger) : base(logger, "OlabFileTag")
  {
  }
}