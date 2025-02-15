using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.WikiTag;
using OLab.Common.Interfaces;
using System.Text;

namespace OLab.Api.ObjectMapper;

public class ScriptsFullMapper : OLabMapper<SystemScripts, ScriptsFullDto>
{
  public ScriptsFullMapper(
    IOLabLogger logger,
    OLabDBContext dbContext,
    WikiTagModuleProvider tagProvider,
    bool enableWikiTranslation = true) : base( logger, dbContext, tagProvider )
  {
  }

  public override ScriptsFullDto PhysicalToDto(
    SystemScripts phys,
    ScriptsFullDto source)
  {
    var dto = base.PhysicalToDto( phys, source );
    //dto.Source = Encoding.ASCII.GetString( phys.Source );
    return dto;
  }

  public override SystemScripts DtoToPhysical(
    ScriptsFullDto dto)
  {
    var phys = base.DtoToPhysical( dto );
    //phys.Source = Encoding.ASCII.GetBytes( dto.Source );
    return phys;
  }


}