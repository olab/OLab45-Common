using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Common.Interfaces;

namespace OLab.Api.ObjectMapper;

public class MapNodeGroupRolesMapper : OLabMapper<MapNodeGrouproles, MapNodeGroupRolesDto>
{
  public MapNodeGroupRolesMapper(
    IOLabLogger logger,
    OLabDBContext dbContext,
    IOLabModuleProvider<IWikiTagModule> tagProvider,
    bool enableWikiTranslation = true) : base(logger, dbContext, tagProvider)
  {
  }

  public override MapNodeGroupRolesDto PhysicalToDto(MapNodeGrouproles phys)
  {
    var dto = base.PhysicalToDto(phys);

    if (phys.Group != null)
      dto.GroupName = phys.Group.Name;

    if (phys.Group != null)
      dto.RoleName = phys.Role.Name;

    return dto;
  }
}
