using Dawn;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;
using OLab.Data.Dtos;
using OLab.Data.Interface;
using OLab.Data.Mappers;
using OLab.Data.ReaderWriters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints;

public partial class UserAuthorizationEndpoint : OLabEndpoint
{
  private readonly UserGroupRolesMapper mapper;

  public UserAuthorizationEndpoint(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext context,
    IOLabModuleProvider<IWikiTagModule> wikiTagProvider,
    IOLabModuleProvider<IFileStorageModule> fileStorageProvider) : base(
      logger,
      configuration,
      context,
      wikiTagProvider,
      fileStorageProvider )
  {
    mapper = new UserGroupRolesMapper( GetLogger(), GetDbContext() );
  }

  /// <summary>
  /// Remove group/role from user
  /// </summary>
  /// <param name="auth">IOLabAuthorization context</param>
  /// <param name="dto">Map group to remove</param>
  /// <param name="token">Cancellation token</param>
  /// <returns>All group/roles for user</returns>
  /// <exception cref="OLabUnauthorizedException"></exception>
  /// <exception cref="OLabObjectNotFoundException"></exception>
  public async Task<IList<UserGroupRolesDto>> DeleteAsync(
    IOLabAuthorization auth,
    UserGroupRolesDto dto,
    CancellationToken token)
  {
    GetLogger().LogInformation( $"UserAuthorizationEndpoint.DeleteAsync()" );

    // test if user has access to parent object
    var accessResult = await auth.HasAccessAsync(
      IOLabAuthorization.AclBitMaskExecute,
      "UserAdmin",
      0 );

    if ( !accessResult )
      throw new OLabUnauthorizedException( "User", dto.UserId );

    var userPhys = await GetDbContext().Users.FirstOrDefaultAsync( x => x.Id == dto.UserId, token );
    if ( userPhys == null )
      throw new OLabObjectNotFoundException( "Users", dto.UserId );

    var mapGroupPhys = userPhys.UserGrouproles
      .FirstOrDefault( x => (x.GroupId == dto.GroupId) && (x.RoleId == dto.RoleId) );

    if ( mapGroupPhys != null )
    {
      userPhys.UserGrouproles.Remove( mapGroupPhys );
      GetDbContext().SaveChanges();

      GetLogger().LogInformation( $"removed group/role {dto.GroupId}/{dto.RoleId} from user {userPhys.Username}" );

    }
    else
      throw new OLabObjectNotFoundException( "UserGroupRole", 0 );

    return mapper.PhysicalToDto( userPhys.UserGrouproles.ToList() );

  }

  /// <summary>
  /// Add group/role to a user
  /// </summary>
  /// <param name="auth">IOLabAuthorization context</param>
  /// <param name="dto">MapGroupsDto</param>
  /// <param name="token">Cancellation token</param>
  /// <returns>All groups for map</returns>
  /// <exception cref="OLabUnauthorizedException"></exception>
  /// <exception cref="OLabObjectNotFoundException"></exception>
  public async Task<IList<UserGroupRolesDto>> AddAsync(
    IOLabAuthorization auth,
    UserGroupRolesDto dto,
    CancellationToken token)

  {
    GetLogger().LogInformation( $"UserAuthorizationEndpoint.AddAsync()" );

    // test if user has access to parent object
    var accessResult = await auth.HasAccessAsync(
      IOLabAuthorization.AclBitMaskExecute,
      "UserAdmin",
      0 );

    if ( !accessResult )
      throw new OLabUnauthorizedException( "User", dto.UserId );

    var userPhys = await GetDbContext().Users.FirstOrDefaultAsync( x => x.Id == dto.UserId, token );
    if ( userPhys == null )
      throw new OLabObjectNotFoundException( "Users", dto.UserId );

    var groupReader = GroupReaderWriter.Instance( GetLogger(), GetDbContext() );
    // ensure group exists
    if ( await groupReader.ExistsAsync( dto.GroupId.ToString() ) )
      throw new OLabObjectNotFoundException( "Group", dto.GroupId );

    var roleReader = RoleReaderWriter.Instance( GetLogger(), GetDbContext() );
    // ensure role exists
    if ( await roleReader.ExistsAsync( dto.RoleId.ToString() ) )
      throw new OLabObjectNotFoundException( "Role", dto.RoleId );

    // test if doesn't already exist
    if ( !userPhys.UserGrouproles.Any( x => (x.RoleId == dto.RoleId) && (x.GroupId == dto.GroupId) ) )
    {
      var userGroupRolePhys = mapper.DtoToPhysical( dto );
      userPhys.UserGrouproles.Add( userGroupRolePhys );

      GetDbContext().SaveChanges();

      GetLogger().LogInformation( $"added group/role {dto.GroupId}/{dto.RoleId} to user {userPhys.Username}" );

    }

    return mapper.PhysicalToDto( userPhys.UserGrouproles.ToList() );

  }

  /// <summary>
  /// Get user's assigned groups
  /// </summary>
  /// <param name="auth">IOLabAuthorization</param>
  /// <param name="token"></param>
  /// <returns></returns>
  /// <exception cref="OLabObjectNotFoundException"></exception>
  public async Task<IList<GroupsDto>> GetUserGroups(IOLabAuthorization auth, CancellationToken token)
  {
    var userId = auth.UserContext.UserId;
    var groupsPhys = new List<Groups>();

    if ( await auth.IsSystemSuperuserAsync() )
    {
      var readerWriter = GroupReaderWriter.Instance( GetLogger(), GetDbContext() );
      groupsPhys.AddRange( await readerWriter.GetAsync() );
    }
    else
    {
      var userPhys = await GetDbContext().Users
        .Include( "UserGrouproles" )
        .FirstOrDefaultAsync( x => x.Id == userId, token );
      if ( userPhys == null )
        throw new OLabObjectNotFoundException( "Users", userId );

      groupsPhys.AddRange( userPhys.UserGrouproles.Select( x => x.Group ).Distinct() );
    }

    var groupsDto = new GroupsMapper(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider() ).PhysicalToDto( groupsPhys );

    return groupsDto;
  }

  /// <summary>
  /// Get all defined users
  /// </summary>
  /// <returns>Enumerable list of users</returns>
  public IEnumerable<Users> GetAll()
  {
    return GetDbContext().Users.ToList();
  }

  /// <summary>
  /// ReadAsync user by Id
  /// </summary>
  /// <param name="id">User id</param>
  /// <returns>User record</returns>
  public Users GetById(uint? id)
  {
    if ( !id.HasValue )
      return null;
    return GetDbContext()
      .Users
      .Include( "UserGrouproles" )
      .FirstOrDefault( x => x.Id == id.Value );
  }

  /// <summary>
  /// Get user by name
  /// </summary>
  /// <param name="userName">User name</param>
  /// <returns>User record</returns>
  public Users GetByUserName(string userName)
  {
    return GetDbContext().Users.FirstOrDefault( x => x.Username.ToLower() == userName.ToLower() );
  }

  /// <summary>
  /// Retrieves a list of users based on the provided name.
  /// </summary>
  /// <param name="name">The name to search for in the users' nicknames or usernames.</param>
  /// <returns>A list of UsersDto objects that match the search criteria.</returns>
  public IList<UsersDto> GetUsers(string name)
  {
    IList<Users> users = new List<Users>();

    if ( !string.IsNullOrEmpty( name ) )
      users = GetDbContext().Users
        .Include( "UserGrouproles" )
        .Include( "UserGrouproles.Group" )
        .Include( "UserGrouproles.Role" )
        .Where( x => x.Nickname.Contains( name ) || x.Username.Contains( name ) ).ToList();
    else
      users = GetDbContext().Users
        .Include( "UserGrouproles" )
        .Include( "UserGrouproles.Group" )
        .Include( "UserGrouproles.Role" )
        .ToList();

    var dtoList = new UsersMapper( GetLogger(), GetDbContext() ).PhysicalToDto( users );
    return dtoList;
  }

  /// <summary>
  /// Edit user based on add user request
  /// </summary>
  /// <param name="userRequest">USer request</param>
  /// <returns>Add user response</returns>
  public async Task<UsersDto> EditUserAsync(AddUserRequest userRequest)
  {
    var user = GetByUserName( userRequest.Username );
    if ( user == null )
      throw new OLabBadRequestException( $"user: '{userRequest.Username}' does not exist" );

    GetLogger().LogInformation( $"editing user '{userRequest.Username}'" );

    // need to set the logger and the dbContext since
    // they are not present when AddUserRequest created by webApi
    userRequest.SetInfrastructure( GetLogger(), GetDbContext() );

    // parse any GroupRole string(s)
    userRequest.BuildGroupRoleObjects();

    // build physical User object from request
    Users.CreatePhysFromRequest( user, userRequest );

    // update and encrypt password if one was passed in
    if ( !string.IsNullOrEmpty( userRequest.Password ) )
      ChangePassword( user, new ChangePasswordRequest
      {
        NewPassword = userRequest.Password
      } );

    GetDbContext().Users.Update( user );
    await GetDbContext().SaveChangesAsync();

    user.UserGrouproles.AddRange( userRequest.GroupRoleObjects );
    GetDbContext().Users.Update( user );
    await GetDbContext().SaveChangesAsync();

    var userDto = new UsersMapper( GetLogger(), GetDbContext() ).PhysicalToDto( user );
    return userDto;
  }


  public async Task<AddUserResponse> DeleteUserAsync(DeleteUsersRequest userRequest)
  {
    GetLogger().LogInformation( $" deleting user '{userRequest.UserName}'" );

    Users user = null;

    // allow for either id or user name to search for
    if ( userRequest.Id > 0 )
      user = GetById( userRequest.Id );
    else if ( !string.IsNullOrEmpty( userRequest.UserName ) )
      user = GetByUserName( userRequest.UserName );

    if ( user == null )
      return new AddUserResponse
      {
        Id = userRequest.Id,
        Error = $"User does not exist"
      };

    var physUser =
      await GetDbContext().Users.FirstOrDefaultAsync( x => x.Id == userRequest.Id );

    GetDbContext().Users.Remove( physUser );
    await GetDbContext().SaveChangesAsync();

    var response = new AddUserResponse
    {
      Id = userRequest.Id
    };

    return response;
  }

  public async Task<List<AddUserResponse>> DeleteUsersAsync(List<DeleteUsersRequest> items)
  {
    try
    {
      var responses = new List<AddUserResponse>();

      GetLogger().LogDebug( $"DeleteUserAsync(items count '{items.Count}')" );

      foreach ( var item in items )
      {
        var response = await DeleteUserAsync( item );
        responses.Add( response );
      }

      return responses;
    }
    catch ( Exception ex )
    {
      GetLogger().LogError( $"DeleteUserAsync exception {ex.Message}" );
      throw;
    }
  }

  /// <summary>
  /// Updates a user record with a new password
  /// </summary>
  /// <param name="user">Existing user record from DB</param>
  /// <param name="model">Change password request model</param>
  /// <returns></returns>
  public void ChangePassword(Users user, ChangePasswordRequest model)
  {
    Guard.Argument( user, nameof( user ) ).NotNull();
    Guard.Argument( model, nameof( model ) ).NotNull();

    var clearText = model.NewPassword;

    // add password salt, if it's defined
    if ( !string.IsNullOrEmpty( user.Salt ) )
      clearText += user.Salt;

    var hash = SHA1.Create();
    var plainTextBytes = Encoding.ASCII.GetBytes( clearText );
    var hashBytes = hash.ComputeHash( plainTextBytes );

    user.Password = BitConverter.ToString( hashBytes ).Replace( "-", "" ).ToLowerInvariant();
  }

  /// <summary>
  /// Add user based on add user request
  /// </summary>
  /// <param name="userRequest">User request</param>
  /// <returns>ADd user response</returns>
  public async Task<UsersDto> AddUserAsync(AddUserRequest userRequest)
  {
    var user = GetByUserName( userRequest.Username );
    if ( user != null )
      throw new OLabBadRequestException( $"'{userRequest.Username}' already exists" );

    GetLogger().LogInformation( $"adding user '{userRequest.Username}'" );

    var newUserPhys = Users.CreatePhysFromRequest( null, userRequest );
    newUserPhys.UserGrouproles.AddRange(
      UserGrouproles.StringToObjectList( GetDbContext(), userRequest.GroupRoles ) );

    // if salt not passed in, then the incoming password is 
    // cleartext, so we need to do a 'change password'
    // on it to convert it to a hash before saving to database.
    if ( string.IsNullOrEmpty( newUserPhys.Salt ) )
      ChangePassword( newUserPhys, new ChangePasswordRequest
      {
        NewPassword = newUserPhys.Password
      } );

    await GetDbContext().Users.AddAsync( newUserPhys );
    await GetDbContext().SaveChangesAsync();

    var userDto = new UsersMapper( GetLogger(), GetDbContext() ).PhysicalToDto( newUserPhys );
    return userDto;
  }

  public async Task<List<UsersDto>> AddUsersAsync(List<AddUserRequest> items)
  {
    try
    {
      var responses = new List<UsersDto>();

      GetLogger().LogDebug( $"AddUserAsync(items count '{items.Count}')" );

      foreach ( var item in items )
      {
        var user = await AddUserAsync( item );
        responses.Add( user );
      }

      return responses;
    }
    catch ( Exception ex )
    {
      GetLogger().LogError( $"AddUserAsync exception {ex.Message}" );
      throw;
    }
  }
}
