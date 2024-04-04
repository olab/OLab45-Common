using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLab.Api.Model;

public partial class Roles
{
  public const string RoleNameSuperuser = "superuser";
  public const string RoleNameAdministrator = "administrator";
  public const string RoleNameAuthor = "author";
  public const string RoleNameLearner = "learner";
  public const string RoleNameModerator = "moderator";
  public const string RoleNameImporter = "importer";

  public static string[] ReservedNames = new string[] {
            RoleNameSuperuser,
            RoleNameAdministrator,
            RoleNameAuthor,
            RoleNameLearner,
            RoleNameModerator,
            RoleNameImporter
        };

  /// <summary>
  /// TEst if group name is reserved
  /// </summary>
  /// <param name="RoleName">Group name to test</param>
  /// <returns></returns>
  public static bool IsReserved(string RoleName)
  {
    return ReservedNames.Contains(RoleName);
  }
}
