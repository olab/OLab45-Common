using Dawn;
using System.Collections.Generic;
using System.Linq;

namespace OLab.Api.Model;

public partial class UserGrouproles
{
  public const string PartSeparator = ":";
  public const string ItemSeparator = ",";

  /// <summary>
  /// Build group/role string 
  /// </summary>
  /// <returns>Group/role string</returns>
  public override string ToString()
  {
    return $"{Group.Name}{PartSeparator}{Role.Name}";
  }

  /// <summary>
  /// Build group/roles string 
  /// </summary>
  /// <param name="items">List of objects</param>
  /// <returns>Combined string</returns>
  public static string ListToString(IList<UserGrouproles> items)
  {
    var groupRoles = new List<string>();

    foreach (var item in items)
      groupRoles.Add(item.ToString());

    return string.Join(ItemSeparator, groupRoles);
  }

  /// <summary>
  /// Build group/role string
  /// </summary>
  /// <param name="groupName">Group name</param>
  /// <param name="roleName">Role name</param>
  /// <returns>String</returns>
  public static string ToString(string groupName, string roleName)
  {
    Guard.Argument(groupName, nameof(groupName)).NotEmpty();
    Guard.Argument(roleName, nameof(roleName)).NotEmpty();

    return $"{groupName}{PartSeparator}{roleName}";
  }

  /// <summary>
  /// Convert a group/role string to object
  /// </summary>
  /// <param name="dbContext">OLabDbContext</param>
  /// <param name="source">Group/role string</param>
  /// <returns>UserGrouproles</returns>
  public static UserGrouproles StringToObject(OLabDBContext dbContext, string source)
  {
    var parts = source.Split(PartSeparator);
    var groupPhys = dbContext.Groups.FirstOrDefault(x => x.Name == parts[0]);
    var rolePhys = dbContext.Roles.FirstOrDefault(x => x.Name == parts[1]);

    if ((groupPhys != null) && (rolePhys != null))
    {
      return new UserGrouproles
      {
        Role = rolePhys,
        RoleId = rolePhys.Id,
        Group = groupPhys,
        GroupId = groupPhys.Id
      };
    }

    return null;
  }

  /// <summary>
  /// Convert a list of group/role strings to list of obects
  /// </summary>
  /// <param name="dbContext">OLabDbContext</param>
  /// <param name="sourceList">Source string of group/role strings</param>
  /// <returns>List of UserGrouproles</returns>
  public static IList<UserGrouproles> StringToObjectList(
    OLabDBContext dbContext,
    string sourceList)
  {
    var items = new List<UserGrouproles>();
    var groupRoleStrings = sourceList.Split(ItemSeparator);

    foreach (var groupRoleString in groupRoleStrings)
    {
      var item = StringToObject(dbContext, groupRoleString);
      if (item != null)
        items.Add(item);
    }

    return items;
  }
}
