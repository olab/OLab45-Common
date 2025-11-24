using DocumentFormat.OpenXml.Office2010.PowerPoint;
using Newtonsoft.Json;
using OLab.Api.Utils;
using OLab.Common.Utils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

public partial class Users
{
  public const string AnonymousUserName = "anonymous";
  public const int PasswordLength = 8;

  [NotMapped]
  public bool? IsHistoryReadonly
  {
    get => HistoryReadonly.HasValue ? HistoryReadonly.Value == 1 : (bool?)null;
    set => HistoryReadonly = value.HasValue ? (value.Value ? (sbyte)1 : (sbyte)0) : (sbyte?)null;
  }

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

  public static string TruncateToJsonObject(Users phys, int maxDepth)
  {
    var json = JsonConvert.SerializeObject(
      new List<Users> { phys },
      new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore } );

    return SerializerUtilities.TruncateJsonToDepth( json, maxDepth + 1 );
  }
}