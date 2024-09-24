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

  public static Users CreatePhysFromRequest(Users sourceUser, AddUserRequest model)
  {
    if (sourceUser == null)
      sourceUser = CreateDefault();

    if (model.Id.HasValue)
      sourceUser.Id = model.Id.Value;

    if (!string.IsNullOrEmpty(model.Salt))
    {
      sourceUser.Salt = model.Salt;
      sourceUser.Password = model.Hash;
    }
    else
    {
      if (!string.IsNullOrEmpty(model.Password))
        sourceUser.Password = model.Password;
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
      Salt = RandomString(SaltLength),
      Password = RandomString(PasswordLength),
      ModeUi = "easy"
    };

    return user;
  }
}