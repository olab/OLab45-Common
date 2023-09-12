using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("FILE")]
public class FileWikiTag : WikiTag1Argument
{
  public FileWikiTag(IOLabLogger logger) : base(logger, "OlabFileTag")
  {
  }
}