using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Common.Interfaces;

namespace OLab.Api.ObjectMapper;

public class Scripts : OLabMapper<SystemScripts, ScriptsDto>
{
  public Scripts(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
  {
  }
}