using Dawn;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using NuGet.Packaging;
using OLab.Access;
using OLab.Access.Interfaces;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
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
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Groups = OLab.Api.Model.Groups;
using Users = OLab.Api.Model.Users;

namespace OLab.Api.Endpoints;

public partial class UserEndpoint : OLabEndpoint
{
  private readonly IOLabMapper<UserGrouproles, UserGroupRolesDto> _mapper;
  private readonly GroupReaderWriter _groupReaderWriter;
  private readonly UserReaderWriter _userReaderWriter;
  private readonly IOLabAuthorization _auth;

  public UserEndpoint(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    IOLabAuthorization auth,
    OLabDBContext context,
    IOLabModuleProvider<IWikiTagModule> wikiTagProvider,
    IOLabModuleProvider<IFileStorageModule> fileStorageProvider) : base(
      logger,
      configuration,
      context,
      wikiTagProvider,
      fileStorageProvider )
  {
    _mapper = new UserGroupRolesMapper( GetLogger(), GetDbContext() );
    _groupReaderWriter = GroupReaderWriter.Instance( GetLogger(), GetDbContext() );
    _userReaderWriter
      = UserReaderWriter.Instance( GetLogger(), GetDbContext() );
    _auth = auth;

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
    UserGroupRolesDto dto,
    CancellationToken token)
  {
    GetLogger().LogInformation( $"UserAuthorizationEndpoint.DeleteAsync()" );

    // test if user has access to parent object
    var accessResult = await _auth.HasAccessAsync(
      IOLabAuthorization.AclBitMaskExecute,
      "UserAdmin",
      0 );

    if ( !accessResult )
      throw new OLabUnauthorizedException( "User", dto.UserId );

    var userPhys = await _userReaderWriter.GetSingleAsync( dto.UserId );
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

    return _mapper.PhysicalToDto( userPhys.UserGrouproles.ToList() );

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
    UserGroupRolesDto dto,
    CancellationToken token)

  {
    GetLogger().LogInformation( $"UserAuthorizationEndpoint.AddAsync()" );

    // test if user has access to parent object
    var accessResult = await _auth.HasAccessAsync(
      IOLabAuthorization.AclBitMaskExecute,
      "UserAdmin",
      0 );

    if ( !accessResult )
      throw new OLabUnauthorizedException( "User", dto.UserId );

    var userPhys = await _userReaderWriter.GetSingleAsync( dto.UserId );
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
      var userGroupRolePhys = _mapper.DtoToPhysical( dto );
      userPhys.UserGrouproles.Add( userGroupRolePhys );

      GetDbContext().SaveChanges();

      GetLogger().LogInformation( $"added group/role {dto.GroupId}/{dto.RoleId} to user {userPhys.Username}" );

    }

    return _mapper.PhysicalToDto( userPhys.UserGrouproles.ToList() );

  }

  /// <summary>
  /// Get user's assigned groups
  /// </summary>
  /// <param name="auth">IOLabAuthorization</param>
  /// <param name="token"></param>
  /// <returns></returns>
  /// <exception cref="OLabObjectNotFoundException"></exception>
  public async Task<IList<GroupsDto>> GetUserGroups(
    CancellationToken token)
  {
    var userId = _auth.OLabUser.Id;
    var groupsPhys = new List<Groups>();

    if ( await _auth.IsSystemSuperuserAsync() )
      groupsPhys.AddRange( await _groupReaderWriter.GetAsync() );
    else
    {
      var userPhys = await _userReaderWriter.GetSingleAsync( userId );
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
  /// Edit user based on add user request
  /// </summary>
  /// <param name="userRequest">USer request</param>
  /// <returns>Add user response</returns>
  public async Task<UsersDto> EditUserAsync(
    AddUserRequest userRequest)
  {
    var physUser = await _userReaderWriter.GetSingleAsync( userRequest.Username );
    if ( physUser == null )
      throw new OLabBadRequestException( $"user: '{userRequest.Username}' does not exist" );

    GetLogger().LogInformation( $"editing user '{userRequest.Username}'" );

    // need to set the logger and the dbContext since
    // they are not present when AddUserRequest created by webApi
    userRequest.SetInfrastructure( GetLogger(), GetDbContext() );

    // parse any GroupRole string(s)
    userRequest.BuildGroupRoleObjects();

    // build physical User object from request
    Users.CreatePhysFromRequest( physUser, userRequest );

    // update and encrypt password if one was passed in
    if ( !string.IsNullOrEmpty( userRequest.Password ) )
      ChangePassword( physUser, new ChangePasswordRequest
      {
        NewPassword = userRequest.Password
      } );

    await _userReaderWriter.UpdateAsync( physUser );

    physUser.UserGrouproles.AddRange( userRequest.GroupRoleObjects );
    await _userReaderWriter.UpdateAsync( physUser );

    var userDto = new UsersMapper( GetLogger(), GetDbContext() ).PhysicalToDto( physUser );
    return userDto;
  }

  public async Task<AddUserResponse> DeleteUserAsync(
    DeleteUsersRequest userRequest)
  {
    GetLogger().LogInformation( $" deleting user '{userRequest.UserName}'" );

    Users physUser = null;

    // allow for either id or user name to search for
    if ( userRequest.Id > 0 )
      physUser = await _userReaderWriter.GetSingleAsync( userRequest.Id );
    else if ( !string.IsNullOrEmpty( userRequest.UserName ) )
      physUser = await _userReaderWriter.GetSingleAsync( userRequest.UserName );

    if ( physUser == null )
      return new AddUserResponse
      {
        Id = userRequest.Id,
        Error = $"User does not exist"
      };

    await _userReaderWriter.DeleteAsync( userRequest.Id );

    var response = new AddUserResponse
    {
      Id = userRequest.Id
    };

    return response;
  }

  public async Task<List<AddUserResponse>> DeleteUsersAsync(
    List<DeleteUsersRequest> items)
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
  public void ChangePassword(
    Users user,
    ChangePasswordRequest model)
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
  public async Task<UsersDto> AddUserAsync(
    AddUserRequest userRequest)
  {
    var user = await _userReaderWriter.GetSingleAsync( userRequest.Username );
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

    newUserPhys = await _userReaderWriter.CreateAsync( newUserPhys );

    var userDto = new UsersMapper( GetLogger(), GetDbContext() ).PhysicalToDto( newUserPhys );
    return userDto;
  }

  public async Task<List<UsersDto>> AddUsersAsync(
    List<AddUserRequest> items)
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

  /// <summary>
  /// Imports users from an Excel file asynchronously.
  /// </summary>
  /// <param name="fileStream">The stream of the Excel file containing user data.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains a list of user import DTOs representing the imported users.</returns>
  /// <exception cref="Exception">Thrown when an error occurs while importing users.</exception>
  public async Task<List<UsersImportDto>> ImportUsersAsync(
    MemoryStream fileStream)
  {
    var responses = new List<UsersImportDto>();
    fileStream.Position = 0;

    using ( var spreadsheetDocument = SpreadsheetDocument.Open( fileStream, false ) )
    {

      var workbookPart =
        spreadsheetDocument.WorkbookPart ?? spreadsheetDocument.AddWorkbookPart();
      var worksheetPart = workbookPart.WorksheetParts.First();
      var sheet = worksheetPart.Worksheet;

      var sstpart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
      var sst = sstpart.SharedStringTable;

      var cells = sheet.Descendants<Cell>();
      var rows = sheet.Descendants<Row>();

      GetLogger().LogInformation( $"Import row count = {rows.LongCount()}" );
      GetLogger().LogInformation( $"       cell count = {cells.LongCount()}" );

      foreach ( var row in rows )
      {
        var column = 0;
        var userRequest = new AddUserRequest(
          GetLogger(),
          GetDbContext() );

        var groupRoleStrings = new List<string>();

        foreach ( var c in row.Elements<Cell>() )
        {
          if ( c.DataType != null && c.DataType == CellValues.SharedString )
          {
            var ssid = int.Parse( c.CellValue.Text );
            var str = sst.ChildElements[ ssid ].InnerText;

            switch ( column )
            {
              case 0:
                userRequest.Operation = str;
                break;
              case 1:
                userRequest.Username = str;
                break;
              case 2:
                userRequest.NickName = str;
                break;
              case 3:
                userRequest.EMail = str;
                break;
              case 4:
                userRequest.Password = str;
                break;
              default:
                groupRoleStrings.Add( str );
                break;
            }

          }

          column++;
        }

        userRequest.GroupRoles = string.Join( ",", groupRoleStrings );

        if ( string.IsNullOrEmpty( userRequest.Operation ) || userRequest.Operation == "+" )
          try
          {
            var response = await AddUserAsync( userRequest );
            responses.Add( new UsersImportDto( response ) { Message = "added" } );
          }
          catch ( Exception ex )
          {
            responses.Add( new UsersImportDto
            {
              UserName = userRequest.Username,
              Status = false,
              Message = ex.Message
            } );
          }

        else if ( userRequest.Operation == "*" )
          try
          {
            var response = await EditUserAsync( userRequest );

            // test if user previously added (in the responses), if so then
            // remove previous before adding edited user
            var existingUser = responses.FirstOrDefault( x => x.Id == response.Id );
            if ( existingUser != null )
            {
              responses.Remove( existingUser );
              responses.Add( new UsersImportDto( response ) { Message = "added, edited" } );
            }
            else
              responses.Add( new UsersImportDto( response ) { Message = "edited" } );

          }
          catch ( Exception ex )
          {
            responses.Add( new UsersImportDto
            {
              UserName = userRequest.Username,
              Status = false,
              Message = ex.Message
            } );
          }

        else if ( userRequest.Operation == "-" )
          try
          {
            var list = new List<DeleteUsersRequest>();
            list.Add( new DeleteUsersRequest { UserName = userRequest.Username } );
            await DeleteUsersAsync( list );

            //responses.Add(new UsersImportDto
            //{
            //  UserName = userRequest.Username,
            //  Message = "deleted"
            //});

          }
          catch ( Exception ex )
          {
            responses.Add( new UsersImportDto
            {
              UserName = userRequest.Username,
              Status = false,
              Message = ex.Message
            } );
          }
      }
    }

    return responses;
  }

  /// <summary>
  /// Retrieves a list of users based on the provided name.
  /// </summary>
  /// <param name="name">The name to search for. If null or empty, all users are returned.</param>
  /// <returns>A list of user DTOs that match the search criteria.</returns>
  public async Task<IList<UsersDto>> GetUsersAsync(
    string name)
  {
    IList<Users> users = new List<Users>();

    if ( !string.IsNullOrEmpty( name ) )
      users = await _userReaderWriter.GetNameLikeAsync( name );
    else
      users = await _userReaderWriter.GetAsync();

    var dtoList = new UsersMapper( GetLogger(), GetDbContext() ).PhysicalToDto( users );
    return dtoList;
  }
}
