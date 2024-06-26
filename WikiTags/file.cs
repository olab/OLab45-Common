using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("FILE")]
public class FileWikiTag : WikiTag1Argument
{
  public FileWikiTag(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base(logger, configuration, "OlabFileTag")
  {
  }
}