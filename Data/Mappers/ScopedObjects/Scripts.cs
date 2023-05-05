using OLabWebAPI.Common;
using OLabWebAPI.Dto;
using OLabWebAPI.Model;
using OLabWebAPI.Utils;

namespace OLabWebAPI.ObjectMapper
{
  public class Scripts : OLabMapper<SystemScripts, ScriptsDto>
  {
    public Scripts(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }
  }
}