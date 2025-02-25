#nullable disable

using Newtonsoft.Json;
using OLab.Common.Utils;
using System;
using System.Collections.Generic;

namespace OLab.Api.Model;

public partial class GrouproleAcls
{
  public const int ReadMask = 4;
  public const int WriteMask = 2;
  public const int ExecuteMask = 1;

  public static GrouproleAcls CreateDefault(
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

  public static string TruncateToJsonObject(GrouproleAcls phys, int maxDepth)
  {
    var json = JsonConvert.SerializeObject(
      new List<GrouproleAcls> { phys },
      new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore } );

    return SerializerUtilities.TruncateJsonToDepth( json, maxDepth + 1 );
  }

  public override string ToString()
  {
    var groupName = (Group != null) ? $"{Group?.Name}({GroupId})" : (GroupId != null ? (GroupId == 0 ? "*" : GroupId.ToString()) : "null");
    var roleName = (Role != null) ? $"{Role?.Name}({RoleId})" : (RoleId != null ? (RoleId == 0 ? "*" : RoleId.ToString()) : "null");
    var imageableType = string.IsNullOrEmpty( ImageableType ) ? "*" : ImageableType;
    var imageableId = ImageableId.HasValue ? $"{string.Join( ',', ImageableId.Value )}" : "null";

    return $"{Id}: {groupName}{UserGrouproles.ItemSeparator}{roleName} {imageableType}({imageableId}) acl: '{Convert.ToString( (int)Acl2, 2 )}'";
  }

}
