using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using AutoMapper.Internal.Mappers;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

public partial class UserGrouproles
{
  public const string Separator = ":";

  public override string ToString()
  {
    return $"{Group.Name}{Separator}{Role.Name}";
  }

  public static string ListToString(IList<UserGrouproles> items)
  {
    var groupRoles = new List<string>();
    foreach (var item in items)
      groupRoles.Add(item.ToString());
    return string.Join(Separator, groupRoles);
  }

  public static IList<UserGrouproles> StringToList(OLabDBContext dbContext, string source)
  {
    var items = new List<UserGrouproles>();
    var GroupRoleStrings = source.Split(",");

    foreach (var item in GroupRoleStrings)
    {
      var parts = item.Split(Separator);
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
