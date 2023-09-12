using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("DL")]
public class DownloadWikiTag : WikiTag1Argument
{
  public DownloadWikiTag(IOLabLogger logger) : base(logger, "OlabDownloadTag")
  {
  }
}