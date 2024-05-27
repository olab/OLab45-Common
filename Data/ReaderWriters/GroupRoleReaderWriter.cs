using OLab.Api.Model;
using Microsoft.Build.Framework;
using System.Linq;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
  /// Create new Group
  /// </summary>
  /// <param name="name">Group name</param>
  /// <returns>Groups</returns>
  public async Task<Groups> CreateGroupAsync(string name)
  {
    var phys = new Groups { Name = name };
    var existing = await GetGroupAsync(name);

    if (existing != null)
    {
      _context.Groups.Add(phys);
      _context.SaveChanges();
    }
    else
      phys = existing;

    return phys;
  }

  /// <summary>
  /// Create new Role
  /// </summary>
  /// <param name="name">Role name</param>
  /// <returns>Roles</returns>
  public async Task<Roles> CreateRoleAsync(string name)
  {
    var phys = new Roles { Name = name };
    var existing = await GetRoleAsync(name);

    if (existing != null)
    {
      _context.Roles.Add(phys);
      _context.SaveChanges();
    }
    else
      phys = existing;

    return phys;
  }

  /// <summary>
  /// Get group by id
  /// </summary>
  /// <param name="id">Group Id</param>
  /// <returns>Groups</returns>
  public async Task<Groups> GetGroupAsync(uint id)
  {
    var phys = await _context.Groups.FirstOrDefaultAsync(x => x.Id == id);
    return phys;
  }

  /// <summary>
  /// Get group by name
  /// </summary>
  /// <param name="name">Group name</param>
  /// <returns>Groups</returns>
  public async Task<Groups> GetGroupAsync(string name)
  {
    var phys = await _context.Groups.FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
    return phys;
  }

  /// <summary>
  /// Get groups
  /// </summary>
  /// <param name="name">Group name</param>
  /// <returns>Groups</returns>
  public async Task<IList<Groups>> GetGroupsAsync(int? take, int? skip)
  {
    if (!take.HasValue && !skip.HasValue)
      return await _context.Groups.ToListAsync();

    if (take.HasValue && skip.HasValue)
      return await _context.Groups.Skip(skip.Value).Take(take.Value).ToListAsync();

    return new List<Groups>();
  }

  /// <summary>
  /// Get role by id
  /// </summary>
  /// <param name="id">Role Id</param>
  /// <returns>Roles</returns>
  public async Task<Roles> GetRoleAsync(uint id)
  {
    var phys = await _context.Roles.FirstOrDefaultAsync(x => x.Id == id);
    return phys;
  }

  /// <summary>
  /// Get role by name
  /// </summary>
  /// <param name="name">Role name</param>
  /// <returns>Roles</returns>
  public async Task<Roles> GetRoleAsync(string name)
  {
    var phys = await _context.Roles.FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
    return phys;
  }

  /// <summary>
  /// Get roles
  /// </summary>
  /// <param name="name">Group name</param>
  /// <returns>Groups</returns>
  public async Task<IList<Roles>> GetRolesAsync(int? take, int? skip)
  {
    if (!take.HasValue && !skip.HasValue)
      return await _context.Roles.ToListAsync();

    if (take.HasValue && skip.HasValue)
      return await _context.Roles.Skip(skip.Value).Take(take.Value).ToListAsync();

    return new List<Roles>();
  }

  /// <summary>
  /// Test if group exists
  /// </summary>
  /// <param name="id">Group id</param>
  /// <returns>true/false</returns>
  public async Task<bool> GroupExistsAsync(
    uint id)
  {
    var phys = await GetGroupAsync(id);
    return phys != null;
  }

  /// <summary>
  /// Test if role exists
  /// </summary>
  /// <param name="id">Role id</param>
  /// <returns>true/false</returns>
  public async Task<bool> RoleExistsAsync(
    uint id)
  {
    var phys = await GetRoleAsync(id);
    return phys != null;
  }

  /// <summary>
  /// Test if group/role pair exists by id
  /// </summary>
  /// <param name="groupId">Group id</param>
  /// <param name="roleId">Role id</param>
  /// <returns>true/false</returns>
  public async Task<bool> ExistsAsync(
    uint groupId,
    uint roleId)
  {
    var haveGroup = GroupExistsAsync(groupId);
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
