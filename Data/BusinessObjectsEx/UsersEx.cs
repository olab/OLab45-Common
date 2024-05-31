using System;
using System.Linq;

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
    return new string(Enumerable.Repeat(chars, length)
        .Select(s => s[random.Next(s.Length)]).ToArray());
  }

  public static Users CreateDefault(AddUserRequest model)
  {
    var newUser = CreateDefault();

    if (!string.IsNullOrEmpty(model.Password))
      newUser.Password = model.Password;

    newUser.Username = model.Username;
    newUser.Nickname = model.NickName;
    newUser.Email = model.EMail;

    foreach (var groupRole in model.GroupRoles)
    {
      newUser.UserGrouproles.Add(
        new UserGrouproles
        {
          GroupId = groupRole.GroupId,
          RoleId = groupRole.RoleId
        }
      );
    }

    return newUser;
  }

  public static Users CreateDefault()
  {
    var user = new Users
    {
      Salt = RandomString(SaltLength),
      Password = RandomString(PasswordLength),
      ModeUi = "easy"
    };

    return user;
  }
}