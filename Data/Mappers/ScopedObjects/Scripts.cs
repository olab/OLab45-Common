using OLab.Api.Dto;
using OLab.Common.Interfaces;
using OLab.Data.BusinessObjects.API;

namespace OLab.Api.ObjectMapper
{
    public class Scripts : OLabMapper<SystemScripts, ScriptsDto>
  {
    public Scripts(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
    {
    }
  }
}