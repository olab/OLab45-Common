#nullable disable

using OLab.Api.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OLab.Api.Model;

public partial class GrouproleAcls
{
  public static GrouproleAcls CreateDefault(
    IUserContext userContext,
    Groups groupPhys,
    Roles rolePhys,
    string imageableType,
    uint imagableId)
  {
    var acl = new GrouproleAcls();
    acl.Group = groupPhys;
    acl.GroupId = groupPhys.Id;
    acl.Role = rolePhys;
    acl.RoleId = rolePhys.Id;

    acl.ImageableType = imageableType;
    acl.ImageableId = imagableId;

    acl.Acl2 = 7;

    return acl;
  }

  public static IList<GrouproleAcls> FindByGroup(
    OLabDBContext dbContext,
    string groupName)
  {
    var items = dbContext.GrouproleAcls
      .Where(x => x.Group.Name == groupName);
    return items.ToList();
  }

  public static GrouproleAcls Find(
    OLabDBContext dbContext,
    string groupName,
    string roleName)
  {
    var groupRolePhys = dbContext.GrouproleAcls
      .FirstOrDefault(x => x.Role.Name == groupName && x.Group.Name == roleName);
    return groupRolePhys;
  }

  public override string ToString()
  {
    return $"{Id}: {Role.Name}{UserGrouproles.ItemSeparator}{Role.Name} {ImageableType}({ImageableId}) '{Convert.ToString((int)Acl2, 2)}'";
  }

}
