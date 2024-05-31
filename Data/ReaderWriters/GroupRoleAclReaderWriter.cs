using OLab.Api.Model;
using System.Linq;
using OLab.Common.Interfaces;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using OLab.Api.Common;
using System;

namespace OLab.Data.ReaderWriters;

public class GroupRoleAclReaderWriter : ReaderWriter
{
  private readonly RoleReaderWriter _roleReaderWriter;

  public static GroupRoleAclReaderWriter Instance(
    IOLabLogger logger,
    OLabDBContext dbContext)
  {
    return new GroupRoleAclReaderWriter(logger, dbContext);
  }

  public GroupRoleAclReaderWriter(
    IOLabLogger logger,
  OLabDBContext dbContext) : base(logger, dbContext)
  {
    _roleReaderWriter = RoleReaderWriter.Instance(GetLogger(), GetDbContext());
  }

  /// <summary>
  /// Create new Group Role Acls
  /// </summary>
  /// <param name="newPhys">GrouproleAcls</param>
  /// <param name="commit">(oprional) commit to db</param>
  /// <returns>Saved GrouproleAcls</returns>
  public async Task<GrouproleAcls> CreateAsync(GrouproleAcls newPhys, bool commit = false)
  {
    await GetDbContext().GrouproleAcls.AddAsync(newPhys);
    if (commit)
      await GetDbContext().SaveChangesAsync();
    return newPhys;
  }

  /// <summary>
  /// Create default group/role ACLs for group
  /// </summary>
  /// <param name="groupId">Group id</param>
  public async Task CreateForGroupAsync(uint groupId)
  {
    var groupRoleAclPhys = new GrouproleAcls
    {
      ImageableId = 0,
      ImageableType = Api.Utils.Constants.ScopeLevelMap,
      GroupId = groupId,
      RoleId = null,
      Acl2 = 5
    };

    await CreateAsync(groupRoleAclPhys);

    var rolePhys = await _roleReaderWriter.GetAsync("author");
    groupRoleAclPhys = new GrouproleAcls
    {
      ImageableId = 0,
      ImageableType = Api.Utils.Constants.ScopeLevelMap,
      GroupId = groupId,
      RoleId = rolePhys.Id,
      Acl2 = 7
    };

    await CreateAsync(groupRoleAclPhys);

    rolePhys = await _roleReaderWriter.GetAsync("superuser");
    groupRoleAclPhys = new GrouproleAcls
    {
      ImageableId = 0,
      ImageableType = Api.Utils.Constants.ScopeLevelMap,
      GroupId = groupId,
      RoleId = rolePhys.Id,
      Acl2 = 7
    };

    await CreateAsync(groupRoleAclPhys);

    rolePhys = await _roleReaderWriter.GetAsync("superuser");
    groupRoleAclPhys = new GrouproleAcls
    {
      ImageableId = 0,
      ImageableType = "Import",
      GroupId = groupId,
      RoleId = rolePhys.Id,
      Acl2 = 7
    };

    await CreateAsync(groupRoleAclPhys);

    rolePhys = await _roleReaderWriter.GetAsync("director");
    groupRoleAclPhys = new GrouproleAcls
    {
      ImageableId = 0,
      ImageableType = Api.Utils.Constants.ScopeLevelMap,
      GroupId = groupId,
      RoleId = rolePhys.Id,
      Acl2 = 7
    };

    await CreateAsync(groupRoleAclPhys);

    rolePhys = await _roleReaderWriter.GetAsync("administrator");
    groupRoleAclPhys = new GrouproleAcls
    {
      ImageableId = 0,
      ImageableType = Api.Utils.Constants.ScopeLevelMap,
      GroupId = groupId,
      RoleId = rolePhys.Id,
      Acl2 = 7
    };

    await CreateAsync(groupRoleAclPhys);

    rolePhys = await _roleReaderWriter.GetAsync("learner");
    groupRoleAclPhys = new GrouproleAcls
    {
      ImageableId = 0,
      ImageableType = Api.Utils.Constants.ScopeLevelMap,
      GroupId = groupId,
      RoleId = rolePhys.Id,
      Acl2 = 5
    };

    await CreateAsync(groupRoleAclPhys);

    rolePhys = await _roleReaderWriter.GetAsync("learner");
    groupRoleAclPhys = new GrouproleAcls
    {
      ImageableId = 0,
      ImageableType = Api.Utils.Constants.ScopeLevelNode,
      GroupId = groupId,
      RoleId = rolePhys.Id,
      Acl2 = 1
    };

    await CreateAsync(groupRoleAclPhys, true);

  }

  /// <summary>
  /// Get list of group role acls for a set of user group roles
  /// </summary>
  /// <param name="groupRoles">List of user group/roles</param>
  /// <returns>List of group role acl records</returns>
  public IList<GrouproleAcls> GetForUser(IList<UserGrouproles> groupRoles)
  {    
    var groupRoleAcls = new List<GrouproleAcls>();

    foreach (var groupRole in groupRoles)
      groupRoleAcls.AddRange(GetDbContext().GrouproleAcls.Where(x => x.GroupId == groupRole.GroupId && x.RoleId == groupRole.RoleId).ToList());

    return groupRoleAcls;
  }
}
