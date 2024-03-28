using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;
using OLab.Api.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OLab.Api.Model;

public partial class Users
{
  public const int SaltLength = 64;
  public const int PasswordLength = 8;
  public const string AnonymousUserName = "anonymous";

  public Users()
  {
  }

  private static readonly Random random = new Random();
  public Users(Users source)
  {
    Username = source.Username;
    UserGroups = source.UserGroups;
    Nickname = source.Nickname;
    Id = source.Id;
  }

  public IList<string> GenerateRoleString()
  {
    var roleStrings = new List<string>();

    foreach (var item in UserGroups)
      roleStrings.Add(Model.UserGroups.GenerateRoleString(item));

    return roleStrings;
  }

  public bool IsAnonymous()
  {
    return UserGroups.Count == 0;
  }

  public static Users GetAnonymousUser(OLabDBContext dbContext)
  {
    var userPhys = dbContext.Users.FirstOrDefault(x => x.Username == AnonymousUserName);
    if (userPhys == null)
      throw new OLabGeneralException($"anonymous user {AnonymousUserName} not configured");
    return userPhys;
  }

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