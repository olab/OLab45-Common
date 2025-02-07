#nullable disable

using OLab.Api.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OLab.Api.Model;

public partial class GrouproleAcls
{
  public const int ReadMask = 4;
  public const int WriteMask = 2;
  public const int ExecuteMask = 1;

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

    acl.Acl2 = ReadMask | WriteMask | ExecuteMask;

    return acl;
  }

  public override string ToString()
  {
    var groupName = (Group != null) ? $"{Group.Name}({Group.Id})" : "NULL";
    var roleName = (Role != null) ? $"{Role.Name}({Role.Id})" : "NULL";
    var imageableType = string.IsNullOrEmpty( ImageableType ) ? "*" : ImageableType;
    var imageableId = ImageableId.HasValue ? $"{ImageableId.Value}" : "*";

    return $"{Id}: {groupName}{UserGrouproles.ItemSeparator}{roleName} {imageableType}({imageableId}) acl: '{Convert.ToString( (int)Acl2, 2 )}'";
  }

}
