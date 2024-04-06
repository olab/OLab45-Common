using OLab.Api.Data.Exceptions;
using OLab.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLab.Api.Model;

public partial class MapGroups
{
    /// <summary>
  /// Create a UserGroups instance from group and role names
  /// </summary>
  /// <param name="dbContext">OLabDbContext</param>
  /// <param name="groupName">Group name</param>
  /// <param name="roleName">Role name</param>
  /// <returns>UserGroups</returns>
  /// <exception cref="OLabObjectNotFoundException">Grup or role name not found</exception>
  public static MapGroups FromGroupNames(
    OLabDBContext dbContext,
    string groupName)
  {
    var groupPhys = dbContext.Groups.FirstOrDefault(x => x.Name == groupName);
    if (groupPhys == null)
      throw new OLabObjectNotFoundException("Groups", groupName);

    return new MapGroups
    {
      Group = groupPhys,
      GroupId = groupPhys.Id
    };
  }

  public static string ToString(IList<MapGroups> groups) 
  {
    var str = string.Join(",", groups.Select(x => x.Group.Name));
    return str;
  }
}
