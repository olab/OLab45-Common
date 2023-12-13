using OLab.Api.Utils;
using OLab.Data.Interface;

#nullable disable

namespace OLab.Api.Models
{
  public partial class SecurityUsers
  {
    public static SecurityUsers CreateDefaultMapACL(IUserContext userContext, Maps map)
    {
      var acl = new SecurityUsers();
      acl.UserId = userContext.UserId;
      acl.Issuer = userContext.Issuer;
      acl.ImageableId = map.Id;
      acl.ImageableType = ConstantStrings.ScopeLevelMap;
      acl.Acl = "RXWD";

      return acl;
    }

  }
}
