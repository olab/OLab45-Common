using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Common.Interfaces;
using OLab.Data.BusinessObjects;

namespace OLab.Api.ObjectMapper
{
    public class ScriptsFull : OLabMapper<SystemScripts, ScriptsFullDto>
  {
    public ScriptsFull(IOLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }
  }
}