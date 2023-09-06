using OLab.Data.Interface;
using OLab.Utils;

#nullable disable

namespace OLab.Model
{
  public partial class SecurityUsers
  {
    public static SecurityUsers CreateDefaultMapACL(IUserContext userContext, Maps map)
    {
      var acl = new SecurityUsers();
      acl.UserId = userContext.UserId;
      acl.Issuer = userContext.Issuer;
      acl.ImageableId = map.Id;
      acl.ImageableType = Constants.ScopeLevelMap;
      acl.Acl = "RXWD";

      return acl;
    }

  }
}
