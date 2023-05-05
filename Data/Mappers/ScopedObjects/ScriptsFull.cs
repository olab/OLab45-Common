using OLabWebAPI.Common;
using OLabWebAPI.Dto;
using OLabWebAPI.Model;
using OLabWebAPI.Utils;

namespace OLabWebAPI.ObjectMapper
{
  public class ScriptsFull : OLabMapper<SystemScripts, ScriptsFullDto>
  {
    public ScriptsFull(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }
  }
}