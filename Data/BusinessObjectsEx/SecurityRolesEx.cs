#nullable disable

using Microsoft.EntityFrameworkCore;
using OLab.Api.Utils;
using System.Linq;

namespace OLab.Api.Model;

public partial class SecurityRoles
{
  public const string RoleSuperuser = "superuser";
  public const string RoleLearner = "learner";
  public const string RoleModerator = "moderator";
  public const string RoleAuthor = "author";

  public SecurityRoles Upsert(
    OLabDBContext dbContext,
    SecurityRoles source,
    Groups groupPhys,
    string role)
  {
    var securityRolePhys = dbContext.SecurityRoles.FirstOrDefault(x =>
      x.GroupId == source.GroupId &&
      x.ImageableId == source.ImageableId &&
      x.ImageableType == Constants.ScopeLevelMap);

    if (securityRolePhys != null)
      dbContext.SecurityRoles.Remove(securityRolePhys);

    // create default map ACL for new group/role
    // only if superuser or author
    if ((role == RoleSuperuser) ||
         (role == RoleAuthor))
    {
      securityRolePhys = new SecurityRoles
      {
        GroupId = groupPhys.Id,
        ImageableId = 0,
        ImageableType = Constants.ScopeLevelMap,
        Acl = "RWXD",
        Role = role
      };

      // create default security role for group
      dbContext.SecurityRoles.Add(securityRolePhys);
      dbContext.SaveChanges();

    }

    return securityRolePhys;

  }

  public override string ToString()
  {
    return $"{Id}: {Role} {ImageableType}({ImageableId}) '{Acl}'";
  }

}
