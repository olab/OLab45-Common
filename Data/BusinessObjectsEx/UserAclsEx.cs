using OLab.Api.Data.Interface;
using OLab.Api.Utils;

#nullable disable

namespace OLab.Api.Model;

public partial class UserAcls
{
  public static UserAcls CreateDefault(IUserContext userContext, Maps map)
  {
    var acl = new UserAcls();
    acl.UserId = userContext.UserId;
    acl.Iss = userContext.Issuer;
    acl.ImageableId = map.Id;
    acl.ImageableType = Constants.ScopeLevelMap;
    acl.Acl = "RXWD";
    acl.Acl2 = 7;

    return acl;
  }

}
