using Microsoft.EntityFrameworkCore;
using OLab.Api.Data.Exceptions;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using OLab.Data.Contracts;
using OLab.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

    var rolePhys = await _roleReaderWriter.GetAsync("author");
    if (rolePhys == null)
      throw new OLabObjectNotFoundException("Roles", "author");

    var groupRoleAclPhys = new GrouproleAcls
    {
      ImageableId = 0,
      ImageableType = Api.Utils.Constants.ScopeLevelNode,
      GroupId = groupId,
      RoleId = rolePhys.Id,
      Acl2 = 7
    };

    await CreateAsync(groupRoleAclPhys);

    rolePhys = await _roleReaderWriter.GetAsync("learner");
    if (rolePhys == null)
      throw new OLabObjectNotFoundException("Roles", "learner");

    groupRoleAclPhys = new GrouproleAcls
    {
      ImageableId = 0,
      ImageableType = Api.Utils.Constants.ScopeLevelMap,
      GroupId = groupId,
      RoleId = rolePhys.Id,
      Acl2 = 5
    };

    await CreateAsync(groupRoleAclPhys);

    groupRoleAclPhys = new GrouproleAcls
    {
      ImageableId = 0,
      ImageableType = Api.Utils.Constants.ScopeLevelNode,
      GroupId = groupId,
      RoleId = rolePhys.Id,
      Acl2 = 5
    };

    await CreateAsync(groupRoleAclPhys, true);


    rolePhys = await _roleReaderWriter.GetAsync("superuser");
    if (rolePhys == null)
      throw new OLabObjectNotFoundException("Roles", "superuser");

    groupRoleAclPhys = new GrouproleAcls
    {
      ImageableId = 0,
      ImageableType = "*",
      GroupId = groupId,
      RoleId = rolePhys.Id,
      Acl2 = 7
    };

    await CreateAsync(groupRoleAclPhys);

    rolePhys = await _roleReaderWriter.GetAsync("director");
    if (rolePhys == null)
      throw new OLabObjectNotFoundException("Roles", "director");

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
    if (rolePhys == null)
      throw new OLabObjectNotFoundException("Roles", "administrator");

    groupRoleAclPhys = new GrouproleAcls
    {
      ImageableId = 0,
      ImageableType = Api.Utils.Constants.ScopeLevelMap,
      GroupId = groupId,
      RoleId = rolePhys.Id,
      Acl2 = 7
    };

    await CreateAsync(groupRoleAclPhys);

  }

  /// <summary>
  /// Get all ACL records
  /// </summary>
  /// <returns>List of group role acl records</returns>
  public async Task<IList<GrouproleAcls>> GetAsync()
  {
    var groupAcls = await GetDbContext()
      .GrouproleAcls
      .Include("Group")
      .Include("Role").ToListAsync();

    return groupAcls;
  }


  /// <summary>
  /// Get list of group role acls for a set of user group roles
  /// </summary>
  /// <param name="groupId">group id or null if 'all'</param>
  /// <param name="roleId">role id or null if 'all'</param>
  /// <param name="objectType">Scoped object type</param>
  /// <param name="objectId">Scoped object id</param>
  /// <returns>List of group role acl records</returns>
  public async Task<IList<GrouproleAcls>> GetAsync(uint? groupId, uint? roleId, string objectType, uint? objectId)
  {
    IList<GrouproleAcls> groupAcls = new List<GrouproleAcls>();

    // handle special case of system default acls
    if ((groupId == null) && (roleId == null) && 
      (string.IsNullOrEmpty(objectType) && !objectId.HasValue))
      groupAcls = await GetDbContext()
        .GrouproleAcls
        .Include("Group")
        .Include("Role")
        .Where(x =>
          x.GroupId == groupId &&
          x.RoleId == roleId).ToListAsync();

    else if ((groupId == null) && (roleId == null))
      groupAcls = await GetDbContext()
        .GrouproleAcls
        .Include("Group")
        .Include("Role")
        .Where(x =>
          x.ImageableType == objectType &&
          x.ImageableId == objectId).ToListAsync();

    else if ((groupId != null) && (roleId == null))
      groupAcls = await GetDbContext()
        .GrouproleAcls
        .Include("Group")
        .Include("Role")
        .Where(x =>
          x.GroupId == groupId.Value).ToListAsync();

    else if ((groupId != null) && (roleId != null))
      groupAcls = await GetDbContext()
        .GrouproleAcls
        .Include("Group")
        .Include("Role")
        .Where(x =>
          x.GroupId == groupId.Value &&
          x.RoleId == roleId.Value).ToListAsync();

    return groupAcls;
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

  /// <summary>
  /// Get list of group role acls for a specific group
  /// </summary>
  /// <param name="groupId">Group id to load</param>
  /// <returns>List of group role acl records</returns>
  public IList<GrouproleAcls> GetForGroup(uint? groupId)
  {
    var groupAcls = GetDbContext()
      .GrouproleAcls
      .Where(x => (x.GroupId == groupId) || (!x.GroupId.HasValue));

    return groupAcls.ToList();

  }

  public async Task<List<DeleteGroupRoleAclResponse>> DeleteGroupRoleAclsAsync(List<uint> aclIds)
  {
    try
    {
      var responses = new List<DeleteGroupRoleAclResponse>();

      GetLogger().LogDebug($"DeleteGroupRoleAclsAsync(items count '{aclIds.Count}')");

      foreach (var aclId in aclIds)
      {
        var response = await DeleteGroupRoleAclAsync(aclId, true);
        responses.Add(response);
      }

      return responses;
    }
    catch (Exception ex)
    {
      GetLogger().LogError($"DeleteGroupRoleAclsAsync exception {ex.Message}");
      throw;
    }
  }

  public async Task<DeleteGroupRoleAclResponse> DeleteGroupRoleAclAsync(uint id, bool commit = false)
  {
    var physAcl =
      await GetDbContext().GrouproleAcls.FirstOrDefaultAsync(x => x.Id == id);

    GetDbContext().GrouproleAcls.Remove(physAcl);
    if ( commit )
      await GetDbContext().SaveChangesAsync();

    var response = new DeleteGroupRoleAclResponse
    {
      Id = id,
      Message = "Deleted"
    };

    return response;
  }
}
