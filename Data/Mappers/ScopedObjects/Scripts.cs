using OLab.Common.Interfaces;
using OLab.Data.Dtos;
using OLab.Data.Models;

namespace OLab.Data.Mappers
{
  public class Scripts : OLabMapper<SystemScripts, ScriptsDto>
  {
    public Scripts(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
    {
    }
  }
}