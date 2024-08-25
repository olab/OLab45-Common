using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;
using OLab.Data.Dtos;
using System.Linq;

namespace OLab.Data.Mappers;

public class GroupRoleAclMapper : OLabMapper<GrouproleAcls, GroupRoleAclDto>
{
  public GroupRoleAclMapper(
    IOLabLogger logger,
    OLabDBContext dbContext) : base(logger, dbContext)
  {
  }

  /// <summary>
  /// Convert a physical object to a specific dto. 
  /// </summary>
  /// <remarks>
  /// Allows for derived class specific overrides that 
  /// don't fit well with default implementation
  /// </remarks>
  /// <param name="phys">Physical object</param>
  /// <param name="source">Base dto object</param>
  /// <returns>Dto object</returns>
  public override GroupRoleAclDto PhysicalToDto(GrouproleAcls phys, GroupRoleAclDto source)
  {
    source.GroupId = phys.GroupId;
    source.GroupName = phys.Group == null ? null : phys.Group.Name;
    source.RoleId = phys.RoleId;
    source.RoleName = phys.Role == null ? null : phys.Role.Name;
    source.ObjectIndex = phys.ImageableId == null ? null : phys.ImageableId;
    source.ObjectType = string.IsNullOrEmpty( phys.ImageableType ) ? null : phys.ImageableType;
    source.Read = ( phys.Acl2 & GrouproleAcls.ReadMask) == GrouproleAcls.ReadMask;
    source.Write = ( phys.Acl2 & GrouproleAcls.WriteMask) == GrouproleAcls.WriteMask;
    source.Execute = ( phys.Acl2 & GrouproleAcls.ExecuteMask) == GrouproleAcls.ExecuteMask;

    return source;
  }

}
