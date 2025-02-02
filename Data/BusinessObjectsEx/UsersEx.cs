using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;

namespace OLab.Api.Model;

public partial class Users
{
  public const string AnonymousUserName = "anonymous";
  public const int SaltLength = 64;
  public const int PasswordLength = 8;
  private static readonly Random random = new Random();

  public static string RandomString(int length)
  {
    const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
    return new string( Enumerable.Repeat( chars, length )
        .Select( s => s[ random.Next( s.Length ) ] ).ToArray() );
  }

  public static Users CreatePhysFromRequest(Users sourceUser, AddUserRequest model)
  {
    if ( sourceUser == null )
      sourceUser = CreateDefault();

    if ( model.Id.HasValue )
      sourceUser.Id = model.Id.Value;

    if ( !string.IsNullOrEmpty( model.Salt ) )
    {
      sourceUser.Salt = model.Salt;
      sourceUser.Password = model.Hash;
    }
    else
    {
      sourceUser.Salt = GenerateRandomString( SaltLength );
      var clearText = model.Password + sourceUser.Salt;

      var hash = SHA1.Create();
      var plainTextBytes = Encoding.ASCII.GetBytes( clearText );
      var hashBytes = hash.ComputeHash( plainTextBytes );
      var encryptedPassword = BitConverter.ToString( hashBytes ).Replace( "-", "" ).ToLowerInvariant();

      sourceUser.Password = encryptedPassword;
    }

    sourceUser.Username = model.Username;
    sourceUser.Nickname = model.NickName;
    sourceUser.Email = model.EMail;
    sourceUser.UserGrouproles.Clear();

    return sourceUser;
  }

  public static Users CreateDefault()
  {
    var user = new Users
    {
      Salt = RandomString( SaltLength ),
      Password = RandomString( PasswordLength ),
      ModeUi = "easy"
    };

    return user;
  }

  /// <summary>
  /// Generates a random string of the specified length using lowercase letters and digits.
  /// </summary>
  /// <param name="length">The length of the random string to generate.</param>
  /// <returns>A random string of the specified length.</returns>
  public static string GenerateRandomString(int length)
  {
    const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
    var random = new Random();
    return new string( Enumerable.Repeat( chars, length )
      .Select( s => s[ random.Next( s.Length ) ] ).ToArray() );
  }
}