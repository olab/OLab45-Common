using OLab.Api.Model;
using Microsoft.Build.Framework;
using System.Linq;
using OLab.Api.Utils;
using OLab.Common.Interfaces;

namespace OLab.Data.ReaderWriters;

public class GroupRoleReaderWriter : ReaderWriter
{
  public static GroupRoleReaderWriter Instance(
    IOLabLogger logger, 
    OLabDBContext context)
  {
    return new GroupRoleReaderWriter(logger, context);
  }

  public GroupRoleReaderWriter(
    IOLabLogger logger, 
    OLabDBContext context) : base(logger, context)
  {
  }

  /// <summary>
  /// Get group by id
  /// </summary>
  /// <param name="id">Group Id</param>
  /// <returns>Groups</returns>
  public Groups GetGroup(uint id)
  {
    var phys = _context.Groups.FirstOrDefault(x => x.Id == id);
    return phys;
  }

  /// <summary>
  /// Get group by name
  /// </summary>
  /// <param name="name">Group name</param>
  /// <returns>Groups</returns>
  public Groups GetGroup(string name)
  {
    var phys = _context.Groups.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
    return phys;
  }

  /// <summary>
  /// Get role by id
  /// </summary>
  /// <param name="id">Role Id</param>
  /// <returns>Roles</returns>
  public Roles GetRole(uint id)
  {
    var phys = _context.Roles.FirstOrDefault(x => x.Id == id);
    return phys;
  }

  /// <summary>
  /// Get role by name
  /// </summary>
  /// <param name="name">Role name</param>
  /// <returns>Roles</returns>
  public Roles GetRole(string name)
  {
    var phys = _context.Roles.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
    return phys;
  }

  /// <summary>
  /// Test if group exists
  /// </summary>
  /// <param name="id">Group id</param>
  /// <returns>true/false</returns>
  public bool GroupExists(
    uint id)
  {
    var phys = GetGroup(id);
    return phys != null;
  }

  /// <summary>
  /// Test if role exists
  /// </summary>
  /// <param name="id">Role id</param>
  /// <returns>true/false</returns>
  public bool RoleExists(
    uint id)
  {
    var phys = GetRole(id);
    return phys != null;
  }

  /// <summary>
  /// Test if group/role pair exists by id
  /// </summary>
  /// <param name="groupId">Group id</param>
  /// <param name="roleId">Role id</param>
  /// <returns>true/false</returns>
  public bool Exists(
    uint groupId,
    uint roleId)
  {
    var haveGroup = GroupExists(groupId);
    var haveRole = RoleExists(roleId);

    if (!haveGroup || !haveRole)
      return false;

    return true;
  }

  /// <summary>
  /// Test if group/role pair exists by name
  /// </summary>
  /// <param name="groupName">Group name</param>
  /// <param name="roleName">Role name</param>
  /// <returns>true/false</returns>
  public bool Exists(
    string groupName,
    string roleName)
  {
    var groupPhys = GetGroup(groupName);
    var rolePhys = GetRole(roleName);

    if ((groupPhys == null) || (rolePhys == null))
      return false;

    return true;
  }

  /// <summary>
  /// Test if group/role pair exists by object
  /// </summary>
  /// <param name="groupName">Group object</param>
  /// <param name="roleName">Role object</param>
  /// <returns>true/false</returns>
  public bool Exists(
    Groups groupPhys,
    Roles rolePhys)
  {
    groupPhys = GetGroup(groupPhys.Id);
    rolePhys = GetRole(rolePhys.Id);

    if ((groupPhys == null) || (rolePhys == null))
      return false;

    return true;
  }

}
