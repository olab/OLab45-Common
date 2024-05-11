#nullable disable

using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace OLab.Api.Model;

public partial class GrouproleAcls
{
  public static GrouproleAcls Find( OLabDBContext dbContext, string groupName, string roleName )
  {
    var groupRolePhys = dbContext.GrouproleAcls
      .FirstOrDefault(x => x.Role.Name == groupName && x.Group.Name == roleName);
    return groupRolePhys;
  }

  public override string ToString()
  {
    return $"{Id}: {Role.Name} {ImageableType}({ImageableId}) '{Acl}'";
  }

}
