using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

public partial class UserGroups
{
  public const string UserGroupOLab = "olab";
  public const string UserGroupAnonymous = "anonymous";
  public const string UserGroupExternal = "external";

  public static IList<UserGroups> ParseRoleString(
    OLabDBContext dbContext,
    string value)
  {
    var userGroups = new List<UserGroups>();

    var roleStrings = value.Split(",");
    foreach (var item in roleStrings)
    {
      var userGroupParts = item.Split(":");
      var groupPhys = dbContext
        .Groups
        .FirstOrDefault(x => x.Id == Convert.ToUInt32(userGroupParts[0]));

      userGroups.Add(new Model.UserGroups
      {
        GroupId = groupPhys == null ? 0 : groupPhys.Id,
        Role = userGroupParts[1]
      });

    }

    return userGroups;
  }

  public static string GenerateRoleString(UserGroups userGroup)
  {
    return $"{userGroup.Group.Name}:{userGroup.Role}";
  }
}
