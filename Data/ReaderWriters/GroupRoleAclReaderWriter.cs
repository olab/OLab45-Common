using OLab.Api.Data.Exceptions;
using OLab.Api.Model;
using OLab.Common.Interfaces;
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
  /// Get list of group role acls for a set of user group roles
  /// </summary>
  /// <param name="groupRoles">List of user group/roles</param>
  /// <returns>List of group role acl records</returns>
  public IList<GrouproleAcls> Get( uint? groupId, uint? roleId, string objectType, uint? objectId )
  {
    var groupRoleAcls = new List<GrouproleAcls>();
    var groupAcls = GetDbContext()
      .GrouproleAcls.Where(x => 
        x.GroupId == groupId && 
        x.RoleId == roleId && 
        x.ImageableType == objectType && 
        x.ImageableId == objectId).ToList();

    return groupRoleAcls;
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
  /// GEt list of group role acls for a specific group
  /// </summary>
  /// <param name="groupId">Group id to load</param>
  /// <returns>List of group role acl records</returns>
  public IList<GrouproleAcls> GetForGroup(uint? groupId)
  {
    var groupAcls = GetDbContext()
      .GrouproleAcls
      .Where(x => (x.GroupId == groupId) || ( !x.GroupId.HasValue ));

    return groupAcls.ToList();

  }
}
