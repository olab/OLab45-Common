using OLab.Common;
using OLab.Dto;
using OLab.Model;
using OLab.Utils;

namespace OLab.ObjectMapper
{
  public class ScriptsFull : OLabMapper<SystemScripts, ScriptsFullDto>
  {
    public ScriptsFull(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }
  }
}