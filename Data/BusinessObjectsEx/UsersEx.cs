using OLab.Api.Utils;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;

namespace OLab.Api.Model;

public partial class Users
{
  public const string AnonymousUserName = "anonymous";
  public const int PasswordLength = 8;

  public static Users CreatePhysFromRequest(Users sourceUser, AddUserRequest model)
  {
    if ( sourceUser == null )
      sourceUser = CreateDefault();

    if ( model.Id.HasValue )
      sourceUser.Id = model.Id.Value;

    sourceUser.Username = model.Username;
    sourceUser.Nickname = model.NickName;
    sourceUser.Email = model.EMail;
    sourceUser.UserGrouproles.Clear();

    return sourceUser;
  }

  public static Users CreateDefault()
  {
    var user = new Users();
    user.Password = StringUtils.GenerateRandomString( PasswordLength );
    user.ModeUi = "easy";
    return user;
  }
}