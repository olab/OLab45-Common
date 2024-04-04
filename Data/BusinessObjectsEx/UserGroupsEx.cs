using DocumentFormat.OpenXml.Spreadsheet;
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

  public static IList<UserGroups> FromString(OLabDBContext dbContext, string source)
  {
    var userGroups = new List<UserGroups>();
    foreach (var item in source.Split(","))
    {
      var itemParts = item.Split(GroupRoleSeparator);
      var groupPhys = dbContext.Groups.FirstOrDefault(x => x.Name == itemParts[0]);
      var rolePhys = dbContext.Roles.FirstOrDefault(x => x.Name == itemParts[1]);

      userGroups.Add(new UserGroups 
      {
        Group = groupPhys,
        GroupId = groupPhys.Id,
        Role = rolePhys,
        RoleId = rolePhys.Id
      });
    }

    return userGroups;
  }

  public static string ToString(IList<UserGroups> userGroups)
  {
    var strings = new List<string>();

    foreach (var item in userGroups)
      strings.Add($"{item.Group.Name}{GroupRoleSeparator}{item.Role.Name}");

    return string.Join(",", strings);

  }
}
