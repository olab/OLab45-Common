#nullable disable

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OLab.Api.Model;

public partial class SecurityRoles
{
  public static IList<SecurityRoles> GetAcls( OLabDBContext dbContect, UserGroups userGroup )
  {
    return dbContect.SecurityRoles
      .Where( x => x.GroupId == userGroup.GroupId && x.RoleId == userGroup.RoleId ).ToList();
  }

  public override string ToString()
  {
    return $"{Id}: {GroupId} {RoleId} {ImageableType}({ImageableId}) '{Acl}'";
  }

}
