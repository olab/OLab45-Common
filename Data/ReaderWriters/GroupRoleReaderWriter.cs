using OLab.Api.Model;
using Microsoft.Build.Framework;
using System.Linq;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using OLab.Api.Common;
using OLab.Api.Dto;
using System;

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
  /// Get groups paged
  /// </summary>
  /// <param name="take">(optional) number of objects to return</param>
  /// <param name="skip">(optional) number of objects to skip</param>
  /// <returns>OLabAPIPagedResponse</returns>
  public async Task<OLabAPIPagedResponse<Groups>> GetGroupsPagedAsync(int? take, int? skip)
  {
    var response = new OLabAPIPagedResponse<Groups>();

    if (!take.HasValue && !skip.HasValue)
    {
      response.Data = await _context.Groups.ToListAsync();
      response.Count = response.Data.Count;
      response.Remaining = 0;
    }

    else if (take.HasValue && skip.HasValue)
    {
      response.Data = await _context.Groups.Skip(skip.Value).Take(take.Value).ToListAsync();
      response.Count += response.Data.Count;
      response.Remaining = _context.Groups.Count() - skip.Value - response.Count;
    }

    else
      _logger.LogWarning($"invalid/partial take/skip parameters");
    
    return response;
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
  /// Get roles paged
  /// </summary>
  /// <param name="take">(optional) number of objects to return</param>
  /// <param name="skip">(optional) number of objects to skip</param>
  /// <returns>OLabAPIPagedResponse</returns>
  public async Task<OLabAPIPagedResponse<Roles>> GetRolesPagedAsync(int? take, int? skip)
  {
    var response = new OLabAPIPagedResponse<Roles>();

    if (!take.HasValue && !skip.HasValue)
    {
      response.Data = await _context.Roles.ToListAsync();
      response.Count = response.Data.Count;
      response.Remaining = 0;
    }

    else if (take.HasValue && skip.HasValue)
    {
      response.Data = await _context.Roles.Skip(skip.Value).Take(take.Value).ToListAsync();
      response.Count += response.Data.Count;
      response.Remaining = _context.Roles.Count() - skip.Value - response.Count;
    }

    else
      _logger.LogWarning($"invalid/partial take/skip parameters");

    return response;
  }

  /// <summary>
  /// Test if group exists by id
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
  /// Test if role exists by id
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
    var haveGroup = await GroupExistsAsync(groupId);
    var haveRole = await RoleExistsAsync(roleId);

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
  public async Task<bool> Exists(
    string groupName,
    string roleName)
  {
    var groupPhys = await GetGroupAsync(groupName);
    var rolePhys = await GetRoleAsync(roleName);

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
  public async Task<bool> ExistsAsync(
    Groups groupPhys,
    Roles rolePhys)
  {
    groupPhys = await GetGroupAsync(groupPhys.Id);
    rolePhys = await GetRoleAsync(rolePhys.Id);

    if ((groupPhys == null) || (rolePhys == null))
      return false;

    return true;
  }

  public async Task DeleteRoleAsync(uint id)
  {
    if (await GroupExistsAsync(id))
    {
      var phys = await _context.Roles.FirstOrDefaultAsync(x => x.Id == id);
      _context.Roles.Remove(phys);
      await _context.SaveChangesAsync();
    }
  }

  public async Task DeleteGroupAsync(uint id)
  {
    if (await GroupExistsAsync(id))
    {
      var phys = await _context.Groups.FirstOrDefaultAsync(x => x.Id == id);
      _context.Groups.Remove(phys);
      await _context.SaveChangesAsync();
    }
  }
}
