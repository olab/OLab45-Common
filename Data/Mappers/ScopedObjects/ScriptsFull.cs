using DocumentFormat.OpenXml.Bibliography;
using Elfie.Serialization;
using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using System.Text;
using OLab.Api.WikiTag;

namespace OLab.Api.ObjectMapper;

public class ScriptsFull : OLabMapper<SystemScripts, ScriptsFullDto>
{
  public ScriptsFull(
    IOLabLogger logger,
    bool enableWikiTranslation = true) : base(logger)
  {
  }
  public ScriptsFull(
    IOLabLogger logger,
    WikiTagModuleProvider tagProvider,
    bool enableWikiTranslation = true) : base(logger, tagProvider)
  {
  }
  public override ScriptsFullDto PhysicalToDto(
    SystemScripts phys,
    ScriptsFullDto source)
  {
    var dto = base.PhysicalToDto(phys, source);
    dto.Source = Encoding.ASCII.GetString(phys.Source);
    return dto;
  }

  public override SystemScripts DtoToPhysical(
    ScriptsFullDto dto)
  {
    var phys = base.DtoToPhysical(dto);
    phys.Source= Encoding.ASCII.GetBytes(dto.Source);
    return phys;
  }


}