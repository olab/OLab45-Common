using System.Collections.Generic;
using System.Linq;
using Dawn;

namespace OLab.Api.Model;

public partial class UserGrouproles
{
  public const string PartSeparator = ":";
  public const string ItemSeparator = ",";

  public override string ToString()
  {
    return $"{Group.Name}{PartSeparator}{Role.Name}";
  }

  public static string ListToString(IList<UserGrouproles> items)
  {
    var groupRoles = new List<string>();
    foreach (var item in items)
      groupRoles.Add(item.ToString());
    return string.Join(ItemSeparator, groupRoles);
  }

  public static string ToString(string groupName, string roleName)
  {
    Guard.Argument(groupName, nameof(groupName)).NotEmpty();
    Guard.Argument(roleName, nameof(roleName)).NotEmpty();

    return $"{groupName}{PartSeparator}{roleName}";
  }


  public static IList<UserGrouproles> StringToList(OLabDBContext dbContext, string source)
  {
    var items = new List<UserGrouproles>();
    var GroupRoleStrings = source.Split(ItemSeparator);

    foreach (var item in GroupRoleStrings)
    {
      var parts = item.Split(PartSeparator);
      var groupPhys = dbContext.Groups.FirstOrDefault(x => x.Name == parts[0]);
      var rolePhys = dbContext.Roles.FirstOrDefault(x => x.Name == parts[1]);

      if ((groupPhys != null) && (rolePhys != null))
      {
        items.Add(new UserGrouproles
        {
          Role = rolePhys,
          RoleId = rolePhys.Id,
          Group = groupPhys,
          GroupId = groupPhys.Id
        });
      }

      return items;
    }


    return items;
  }
}
