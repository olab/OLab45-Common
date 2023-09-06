using OLab.Api.Common;
using OLab.Api.Utils;

[WikiTagModule("DL")]
public class DownloadWikiTag : WikiTag1Argument
{
  public DownloadWikiTag(OLabLogger logger) : base(logger, "OlabDownloadTag")
  {
  }
}