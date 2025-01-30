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
    bool enableWikiTranslation = true) : base( logger, dbContext, tagProvider )
  {
  }

  public override MapNodeGroupRolesDto PhysicalToDto(
    MapNodeGrouproles phys,
    MapNodeGroupRolesDto dto)
  {
    dto.Id = phys.Id;
    dto.NodeId = phys.NodeId;

    if ( phys.Group != null )
      dto.GroupName = phys.Group.Name;

    if ( phys.Role != null )
      dto.RoleName = phys.Role.Name;

    return dto;
  }

  public override MapNodeGrouproles DtoToPhysical(
    MapNodeGroupRolesDto dto,
    MapNodeGrouproles phys)
  {
    phys.NodeId = dto.NodeId;

    if ( dto.Id.HasValue )
      phys.Id = dto.Id.Value;

    if ( dto.GroupId == null )
      phys.Group = null;

    if ( dto.RoleId == null )
      phys.Role = null;

    return phys;
  }

}
