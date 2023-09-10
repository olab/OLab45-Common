using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Common.Attributes;

[OLabModule("DL")]
public class DownloadWikiTag : WikiTag1Argument
{
  public DownloadWikiTag(OLabLogger logger) : base(logger, "OlabDownloadTag")
  {
  }
}