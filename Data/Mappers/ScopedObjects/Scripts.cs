using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;

namespace OLab.Api.ObjectMapper
{
    public class Scripts : OLabMapper<SystemScripts, ScriptsDto>
  {
    public Scripts(IOLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }
  }
}