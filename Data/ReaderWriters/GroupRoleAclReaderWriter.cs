using Microsoft.EntityFrameworkCore;
using OLab.Api.Data.Exceptions;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using OLab.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Data.ReaderWriters;

/// <summary>
/// Provides methods to read and write Group Role ACLs.
/// </summary>
public class GroupRoleAclReaderWriter : ReaderWriter
{
  private readonly RoleReaderWriter _roleReaderWriter;

  /// <summary>
  /// Creates an instance of <see cref="GroupRoleAclReaderWriter"/>.
  /// </summary>
  /// <param name="logger">The logger instance.</param>
  /// <param name="dbContext">The database context.</param>
  /// <returns>An instance of <see cref="GroupRoleAclReaderWriter"/>.</returns>
  public static GroupRoleAclReaderWriter Instance(
    IOLabLogger logger,
    OLabDBContext dbContext)
  {
    return new GroupRoleAclReaderWriter( logger, dbContext );
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="GroupRoleAclReaderWriter"/> class.
  /// </summary>
  /// <param name="logger">The logger instance.</param>
  /// <param name="dbContext">The database context.</param>
  public GroupRoleAclReaderWriter(
    IOLabLogger logger,
    OLabDBContext dbContext) : base( logger, dbContext )
  {
    _roleReaderWriter = RoleReaderWriter.Instance( GetLogger(), GetDbContext() );
  }

  /// <summary>
  /// Edit a new Group Role ACL.
  /// </summary>
  /// <param name="newPhys">The new Group Role ACL to create.</param>
  /// <returns>The edited Group Role ACL.</returns>
  public async Task<GrouproleAcls> EditAsync(GrouproleAcls newPhys, bool commit = false)
  {
    GetDbContext().GrouproleAcls.Update( newPhys );
    if ( commit )
      await GetDbContext().SaveChangesAsync();
    return newPhys;
  }

  /// <summary>
  /// Creates a new Group Role ACL.
  /// </summary>
  /// <param name="newPhys">The new Group Role ACL to create.</param>
  /// <param name="commit">Whether to commit the changes to the database.</param>
  /// <returns>The created Group Role ACL.</returns>
  public async Task<GrouproleAcls> CreateAsync(GrouproleAcls newPhys, bool commit = false)
  {
    await GetDbContext().GrouproleAcls.AddAsync( newPhys );
    if ( commit )
      await GetDbContext().SaveChangesAsync();
    return newPhys;
  }

  /// <summary>
  /// Creates default group/role ACLs for a group.
  /// </summary>
  /// <param name="groupId">The ID of the group.</param>
  public async Task CreateDefaultAclsForGroupAsync(uint groupId)
  {
    var rolePhys = await _roleReaderWriter.GetAsync( "author" );
    if ( rolePhys == null )
      throw new OLabObjectNotFoundException( "Roles", "author" );

    var groupRoleAclPhys = new GrouproleAcls
    {
      ImageableId = 0,
      ImageableType = Api.Utils.Constants.ScopeLevelNode,
      GroupId = groupId,
      RoleId = rolePhys.Id,
      Acl2 = 7
    };

    await CreateAsync( groupRoleAclPhys );

    rolePhys = await _roleReaderWriter.GetAsync( "learner" );
    if ( rolePhys == null )
      throw new OLabObjectNotFoundException( "Roles", "learner" );

    groupRoleAclPhys = new GrouproleAcls
    {
      ImageableId = 0,
      ImageableType = Api.Utils.Constants.ScopeLevelMap,
      GroupId = groupId,
      RoleId = rolePhys.Id,
      Acl2 = 5
    };

    await CreateAsync( groupRoleAclPhys );

    groupRoleAclPhys = new GrouproleAcls
    {
      ImageableId = 0,
      ImageableType = Api.Utils.Constants.ScopeLevelNode,
      GroupId = groupId,
      RoleId = rolePhys.Id,
      Acl2 = 5
    };

    await CreateAsync( groupRoleAclPhys, true );

    rolePhys = await _roleReaderWriter.GetAsync( Roles.SuperUserRole );
    if ( rolePhys == null )
      throw new OLabObjectNotFoundException( "Roles", Roles.SuperUserRole );

    groupRoleAclPhys = new GrouproleAcls
    {
      ImageableId = 0,
      ImageableType = "*",
      GroupId = groupId,
      RoleId = rolePhys.Id,
      Acl2 = 7
    };

    await CreateAsync( groupRoleAclPhys );

    rolePhys = await _roleReaderWriter.GetAsync( Roles.DirectorRole );
    if ( rolePhys == null )
      throw new OLabObjectNotFoundException( "Roles", Roles.DirectorRole );

    groupRoleAclPhys = new GrouproleAcls
    {
      ImageableId = 0,
      ImageableType = Api.Utils.Constants.ScopeLevelMap,
      GroupId = groupId,
      RoleId = rolePhys.Id,
      Acl2 = 7
    };

    await CreateAsync( groupRoleAclPhys );

    rolePhys = await _roleReaderWriter.GetAsync( "administrator" );
    if ( rolePhys == null )
      throw new OLabObjectNotFoundException( "Roles", "administrator" );

    groupRoleAclPhys = new GrouproleAcls
    {
      ImageableId = 0,
      ImageableType = Api.Utils.Constants.ScopeLevelMap,
      GroupId = groupId,
      RoleId = rolePhys.Id,
      Acl2 = 7
    };

    await CreateAsync( groupRoleAclPhys );
  }

  /// <summary>
  /// Gets all Group Role ACL records.
  /// </summary>
  /// <returns>A list of Group Role ACL records.</returns>
  public async Task<IList<GrouproleAcls>> GetAsync()
  {
    var groupAcls = await GetDbContext()
      .GrouproleAcls
      .Include( "Group" )
      .Include( "Role" ).ToListAsync();

    return groupAcls;
  }

  /// <summary>
  /// Gets a list of Group Role ACLs for a set of user group roles.
  /// </summary>
  /// <param name="groupId">Group Id: null = ignore, 0 = null, else id</param>
  /// <param name="roleId">Role Id: null = ignore, 0 = null, else id</param>
  /// <param name="objectType">Object types: null = ignore, multiple comma delimited</param>
  /// <param name="objectIds">List of id's: null = ignore, 0 = null, else id</param>
  /// <returns>A list of Group Role ACL records.</returns>
  public async Task<IList<GrouproleAcls>> GetListAsync(
    uint? groupId,
    uint? roleId = null,
    string objectType = null,
    IList<uint?> objectIds = null)
  {
    IList<GrouproleAcls> groupAcls = new List<GrouproleAcls>();

    // if no object type, change this to null
    objectType = string.IsNullOrEmpty( objectType ) ? null : objectType;

    var query = GetDbContext().GrouproleAcls
      .Include( "Group" )
      .Include( "Role" )
      .Select( f => f );

    if ( groupId.HasValue )
    {
      if ( groupId.Value == 0 )
        query = query.Where( x => x.GroupId.HasValue );
      else
        query = query.Where( x => x.GroupId.HasValue && x.GroupId.Value == groupId.Value );
    }

    if ( roleId.HasValue )
    {
      if ( roleId.Value == 0 )
        query = query.Where( x => x.RoleId.HasValue );
      else
        query = query.Where( x => x.RoleId.HasValue && x.RoleId.Value == roleId.Value );
    }

    if ( !string.IsNullOrEmpty( objectType ) )
    {
      query = query.Where( x => !string.IsNullOrEmpty( x.ImageableType ) && objectType.Contains( x.ImageableType ) );

      if ( objectIds != null )
      {
        if ( objectIds.Contains( 0 ) )
          query = query.Where( x => x.ImageableId.HasValue );
        else
          query = query.Where( x => x.ImageableId.HasValue && objectIds.Contains( x.ImageableId.Value ) );
      }
    }

    groupAcls = await query.ToListAsync();

    return groupAcls.Distinct().ToList();
  }

  /// <summary>
  /// Gets a list of Group Role ACLs for a set of user group roles.
  /// </summary>
  /// <param name="groupRoles">The list of user group/roles.</param>
  /// <returns>A list of Group Role ACL records.</returns>
  public IList<GrouproleAcls> GetByUserGroupRoles(IList<UserGrouproles> groupRoles)
  {
    var groupRoleAcls = new List<GrouproleAcls>();

    foreach ( var groupRole in groupRoles )
      groupRoleAcls.AddRange( GetDbContext().GrouproleAcls.Where( x => x.GroupId == groupRole.GroupId && x.RoleId == groupRole.RoleId ).ToList() );

    return groupRoleAcls;
  }

  /// <summary>
  /// Gets a list of Group Role ACLs for a specific group.
  /// </summary>
  /// <param name="groupId">The group ID to load.</param>
  /// <returns>A list of Group Role ACL records.</returns>
  public IList<GrouproleAcls> GetForGroup(uint? groupId)
  {
    var groupAcls = GetDbContext()
      .GrouproleAcls
      .Where( x => (x.GroupId == groupId) || (!x.GroupId.HasValue) );

    return groupAcls.ToList();
  }

  /// <summary>
  /// Deletes a list of Group Role ACLs asynchronously.
  /// </summary>
  /// <param name="aclIds">The list of ACL IDs to delete.</param>
  /// <returns>A list of responses for the deleted ACLs.</returns>
  public async Task<List<DeleteGroupRoleAclResponse>> DeleteAsync(
    List<uint> aclIds)
  {
    try
    {
      var responses = new List<DeleteGroupRoleAclResponse>();

      GetLogger().LogDebug( $"DeleteGroupRoleAclsAsync(items count '{aclIds.Count}')" );

      foreach ( var aclId in aclIds )
      {
        var response = await DeleteAsync( aclId, true );
        responses.Add( response );
      }

      return responses;
    }
    catch ( Exception ex )
    {
      GetLogger().LogError( $"DeleteGroupRoleAclsAsync exception {ex.Message}" );
      throw;
    }
  }

  /// <summary>
  /// Get ACL by id
  /// </summary>
  /// <param name="id">Id to get</param>
  /// <param name="commit">optional commit</param>
  /// <returns></returns>
  public async Task<GrouproleAcls> GetAsync(
    uint id)
  {
    var physAcl =
      await GetDbContext().GrouproleAcls.FirstOrDefaultAsync( x => x.Id == id );
    if ( physAcl == null )
      throw new OLabObjectNotFoundException( "GroupRoleAcl", id );

    return physAcl;
  }

  /// <summary>
  /// Deletes a Group Role ACL asynchronously.
  /// </summary>
  /// <param name="id">The ID of the ACL to delete.</param>
  /// <param name="commit">Whether to commit the changes to the database.</param>
  /// <returns>A response for the deleted ACL.</returns>
  public async Task<DeleteGroupRoleAclResponse> DeleteAsync(
    uint id,
    bool commit = false)
  {
    var physAcl =
      await GetDbContext().GrouproleAcls.FirstOrDefaultAsync( x => x.Id == id );

    GetDbContext().GrouproleAcls.Remove( physAcl );
    if ( commit )
      await GetDbContext().SaveChangesAsync();

    var response = new DeleteGroupRoleAclResponse
    {
      Id = id,
      Message = "Deleted"
    };

    return response;
  }

  /// <summary>
  /// Finds a Group Role ACL by group name and role name.
  /// </summary>
  /// <param name="groupName">The name of the group.</param>
  /// <param name="roleName">The name of the role.</param>
  /// <returns>The found Group Role ACL.</returns>
  public GrouproleAcls GetByGroupRole(
    string groupName,
    string roleName)
  {
    var groupRolePhys
      = GetDbContext().GrouproleAcls
        .FirstOrDefault( x => x.Role != null && x.Role.Name == roleName && x.Group != null && x.Group.Name == groupName );
    return groupRolePhys;
  }

  /// <summary>
  /// Finds Group Role ACLs by group name.
  /// </summary>
  /// <param name="groupName">The name of the group or null to find ACLs without a group.</param>
  /// <returns>A list of Group Role ACLs.</returns>
  public IList<GrouproleAcls> GetByGroup(
    string groupName = null)
  {
    if ( string.IsNullOrEmpty( groupName ) )
    {
      var items = GetDbContext().GrouproleAcls
        .Where( x => !x.GroupId.HasValue );
      return items.ToList();
    }
    else
    {
      var items = GetDbContext().GrouproleAcls
        .Where( x => x.Group != null && x.Group.Name == groupName );
      return items.ToList();
    }
  }

  /// <summary>
  /// Finds Group Role ACLs by role name.
  /// </summary>
  /// <param name="roleName">The name of the role.</param>
  /// <returns>A list of Group Role ACLs.</returns>
  public IList<GrouproleAcls> GetByRole(
    string roleName)
  {
    if ( string.IsNullOrEmpty( roleName ) )
    {
      var items = GetDbContext().GrouproleAcls
        .Where( x => !x.RoleId.HasValue );
      return items.ToList();
    }
    else
    {
      var items = GetDbContext().GrouproleAcls
        .Where( x => x.Role != null && x.Role.Name == roleName );
      return items.ToList();
    }
  }

}
