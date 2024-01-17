using OLab.Api.Common;
using OLab.Common.Interfaces;
using OLab.Data.Dtos;
using OLab.Data.Models;

namespace OLab.Data.Mappers;

public class ScriptsFull : OLabMapper<SystemScripts, ScriptsFullDto>
{
  public ScriptsFull(IOLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
  {
  }
}