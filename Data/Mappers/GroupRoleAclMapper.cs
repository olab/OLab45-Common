using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;

namespace OLab.Data.Mappers;

public class GroupRoleAclMapper : OLabMapper<GrouproleAcls, GroupRoleAclDto>
{
  public GroupRoleAclMapper(
    IOLabLogger logger,
    OLabDBContext dbContext) : base( logger, dbContext )
  {
  }

  /// <summary>
  /// Convert a physical object to a specific dto. 
  /// </summary>
  /// <remarks>
  /// Allows for derived class specific overrides that 
  /// don't fit well with default implementation
  /// </remarks>
  /// <param name="from">Physical object</param>
  /// <param name="to">Base dto object</param>
  /// <returns>Dto object</returns>
  public override GroupRoleAclDto PhysicalToDto(GrouproleAcls from, GroupRoleAclDto to)
  {
    to.Id = from.Id;
    to.GroupId = from.GroupId;
    to.GroupName = from.Group == null ? null : from.Group.Name;
    to.RoleId = from.RoleId;
    to.RoleName = from.Role == null ? null : from.Role.Name;
    to.ObjectIndex = from.ImageableId == null ? null : from.ImageableId;
    to.ObjectType = string.IsNullOrEmpty( from.ImageableType ) ? null : from.ImageableType;
    to.Read = (from.Acl2 & GrouproleAcls.ReadMask) == GrouproleAcls.ReadMask;
    to.Write = (from.Acl2 & GrouproleAcls.WriteMask) == GrouproleAcls.WriteMask;
    to.Execute = (from.Acl2 & GrouproleAcls.ExecuteMask) == GrouproleAcls.ExecuteMask;

    return to;
  }

  /// <summary>
  /// Convert a object to a specific physical . 
  /// </summary>
  /// <param name="from"></param>
  /// <param name="to"></param>
  /// <returns>physical object</returns>
  public override GrouproleAcls DtoToPhysical(GroupRoleAclDto from, GrouproleAcls to)
  {
    if ( from.Id.HasValue )
      to.Id = from.Id.Value;
    to.GroupId = from.GroupId;
    to.Group = null;
    to.RoleId = from.RoleId;
    to.Role = null;
    to.ImageableId = from.ObjectIndex;
    to.ImageableType = from.ObjectType;
    to.Acl2 = from.Read ? (ulong)GrouproleAcls.ReadMask : 0;
    to.Acl2 |= from.Write ? (ulong)GrouproleAcls.WriteMask : 0;
    to.Acl2 |= from.Execute ? (ulong)GrouproleAcls.ExecuteMask : 0;

    return to;
  }

}
