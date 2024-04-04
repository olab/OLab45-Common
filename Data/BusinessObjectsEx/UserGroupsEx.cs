using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Data.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OLab.Api.Model;

public partial class UserGroups
{
  public const string GroupRoleSeparator = "::";

  /// <summary>
  /// Create a UserGroups instance from group and role names
  /// </summary>
  /// <param name="dbContext">OLabDbContext</param>
  /// <param name="groupName">Group name</param>
  /// <param name="roleName">Role name</param>
  /// <returns>UserGroups</returns>
  /// <exception cref="OLabObjectNotFoundException">Grup or role name not found</exception>
  public static UserGroups FromGroupRoleNames(
    OLabDBContext dbContext,
    string groupName,
    string roleName)
  {
    var groupPhys = dbContext.Groups.FirstOrDefault(x => x.Name == groupName);
    if (groupPhys == null)
      throw new OLabObjectNotFoundException("Groups", groupName);

    var rolePhys = dbContext.Roles.FirstOrDefault(x => x.Name == roleName);
    if (groupPhys == null)
      throw new OLabObjectNotFoundException("Roles", roleName);

    return new UserGroups
    {
      Group = groupPhys,
      GroupId = groupPhys.Id,
      Role = rolePhys,
      RoleId = rolePhys.Id
    };
  }

  /// <summary>
  /// Create list of UserGroups records from a string
  /// </summary>
  /// <param name="dbContext">OLabDbContext</param>
  /// <param name="source">Source string</param>
  /// <returns></returns>
  public static IList<UserGroups> FromString(OLabDBContext dbContext, string source)
  {
    var userGroups = new List<UserGroups>();
    foreach (var item in source.Split(",").Distinct())
    {
      var itemParts = item.Split(GroupRoleSeparator);
      userGroups.Add( FromGroupRoleNames( dbContext, itemParts[0], itemParts[1]));
    }

    return userGroups;
  }

  /// <summary>
  /// Create string from list of UserGroups records
  /// </summary>
  /// <param name="userGroups">Source string</param>
  /// <returns></returns>
  public static string ToString(IList<UserGroups> userGroups)
  {
    var strings = new List<string>();

    foreach (var item in userGroups)
      strings.Add($"{item.Group.Name}{GroupRoleSeparator}{item.Role.Name}");

    return string.Join(",", strings);

  }
}
