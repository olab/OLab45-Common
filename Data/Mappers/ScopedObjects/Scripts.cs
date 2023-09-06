using OLab.Common;
using OLab.Dto;
using OLab.Model;
using OLab.Utils;

namespace OLab.ObjectMapper
{
  public class Scripts : OLabMapper<SystemScripts, ScriptsDto>
  {
    public Scripts(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }
  }
}