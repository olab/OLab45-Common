using Microsoft.EntityFrameworkCore;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Data.ReaderWriters;

public class UserReaderWriter : ReaderWriter
{
  public static UserReaderWriter Instance(
    IOLabLogger logger,
    OLabDBContext context)
  {
    return new UserReaderWriter( logger, context );
  }

  public UserReaderWriter(
    IOLabLogger logger,
    OLabDBContext context) : base( logger, context )
  {
  }

  public async Task<Users> GetSingleAsync(string name)
  {
    var physUser = await GetDbContext().Users
      .Include( x => x.UserGrouproles ).ThenInclude( y => y.Group )
      .Include( x => x.UserGrouproles ).ThenInclude( y => y.Role )
      .FirstOrDefaultAsync( x => x.Username.ToLower() == name.ToLower() );

    return physUser;
  }

  public async Task<Users> GetSingleAsync(uint id)
  {
    var physUser = await GetDbContext().Users
      .Include( x => x.UserGrouproles ).ThenInclude( y => y.Group )
      .Include( x => x.UserGrouproles ).ThenInclude( y => y.Role )
      .FirstOrDefaultAsync( x => x.Id == id );

    return physUser;
  }

  public async Task<IList<Users>> GetNameLikeAsync(string name)
  {
    var physUsers = await GetDbContext().Users
      .Include( "UserGrouproles" )
      .Include( "UserGrouproles.Group" )
      .Include( "UserGrouproles.Role" )
      .Where( x => x.Nickname.Contains( name ) || x.Username.Contains( name ) ).ToListAsync();

    return physUsers;
  }

  public async Task<IList<Users>> GetAsync()
  {
    var physUsers = await GetDbContext().Users
        .Include( "UserGrouproles" )
        .Include( "UserGrouproles.Group" )
        .Include( "UserGrouproles.Role" )
        .ToListAsync();

    return physUsers;
  }

  public async Task<Users> CreateAsync(Users physUser)
  {
    var existingUser = await GetSingleAsync( physUser.Username );
    if ( existingUser == null )
    {
      GetLogger().LogInformation( $"creating user '{physUser.Username}'" );
      GetDbContext().Users.Add( physUser );
      GetDbContext().SaveChanges();
    }
    else
      physUser = null;

    return physUser;
  }

  public async Task<bool> DeleteAsync(uint id)
  {
    var physUser = await GetSingleAsync( id );
    if ( physUser == null )
    {
      GetLogger().LogInformation( $"user '{id}' does not exist for deletion" );
      return false;
    }

    GetLogger().LogInformation( $"deleting user '{physUser.Username}'" );

    GetDbContext().Users.Remove( physUser );
    await GetDbContext().SaveChangesAsync();

    return true;
  }

  public async Task DeleteAsync(Users physUser)
  {
    GetLogger().LogInformation( $"deleting user '{physUser.Username}'" );

    GetDbContext().Users.Remove( physUser );
    await GetDbContext().SaveChangesAsync();
  }

  public async Task<Users> UpdateAsync(Users physUser)
  {
    GetLogger().LogInformation( $"updating user '{physUser.Username}'" );

    GetDbContext().Users.Update( physUser );
    await GetDbContext().SaveChangesAsync();

    return physUser;
  }

}
