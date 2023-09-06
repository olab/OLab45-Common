using OLab.Common;
using OLab.Utils;

[WikiTagModule("DL")]
public class DownloadWikiTag : WikiTag1Argument
{
  public DownloadWikiTag(OLabLogger logger) : base(logger, "OlabDownloadTag")
  {
  }
}