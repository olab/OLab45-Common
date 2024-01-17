using OLab.Common.Interfaces;
using OLab.Data.Dtos;
using OLab.Data.Models;

namespace OLab.Data.Mappers;

public class ScriptsMapper : OLabMapper<SystemScripts, ScriptsDto>
{
  public ScriptsMapper(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
  {
  }
}