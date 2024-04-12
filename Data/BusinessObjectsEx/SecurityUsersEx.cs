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
  public const string ReadChar = "R";
  public const string WriteChar = "W";
  public const string ExecuteChar = "X";

  public const uint Read = 0b100;
  public const uint Write = 0b010;
  public const uint Execute = 0b001;
  public const uint NoAccess = 0b000;
  public const uint AllAccess = 0b111;

  public static ulong AclStringToBitMask(string aclString)
  {
    uint bitMask = 0b0;
    aclString = aclString.ToUpper();

    if (aclString.Contains(ReadChar))
      bitMask |= Read;

    if (aclString.Contains(WriteChar))
      bitMask |= Write;

    if (aclString.Contains(ExecuteChar))
      bitMask |= Execute;

    return bitMask;
  }

  public static string BitMaskToAclString(ulong acl)
  {
    string aclString = string.Empty;

    if ((acl & Read) == Read)
      aclString += ReadChar;

    if ((acl & Write) == Write)
      aclString += WriteChar;

    if ((acl & Execute) == Execute)
      aclString += ExecuteChar;

    return aclString;
  }

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
    acl.Acl2 = SecurityRoles.AllAccess;

    return acl;
  }

}
