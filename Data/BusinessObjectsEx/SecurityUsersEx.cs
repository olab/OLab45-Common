using Microsoft.EntityFrameworkCore;
using OLab.Api.Data.Interface;
using OLab.Api.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable disable

namespace OLab.Api.Model;

public partial class SecurityUsers
{
  public const uint Read = 0b100;
  public const uint Write = 0b010;
  public const uint Execute = 0b001;

  public static IList<SecurityUsers> GetAcls(OLabDBContext dbContext, uint userId)
  {
    return dbContext.SecurityUsers.Where(x => x.UserId == userId).ToList();
  }

  public static SecurityUsers CreateDefaultMapACL(IUserContext userContext, Maps map)
  {
    var acl = new SecurityUsers();
    acl.UserId = userContext.UserId;
    acl.Iss = userContext.Issuer;
    acl.ImageableId = map.Id;
    acl.ImageableType = Constants.ScopeLevelMap;
    acl.Acl = "RXWD";

    return acl;
  }

}
