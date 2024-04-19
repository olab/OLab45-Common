using Humanizer;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLab.Api.Model;

public partial class Groups
{
  public const string GroupNameAnonymous = "anonymous";
  public const string GroupNameOLab = "olab";
  public const string GroupNameExternal = "external";

  public static string[] ReservedNames = new string[] {
            GroupNameAnonymous,
            GroupNameOLab,
            GroupNameExternal,
        };

  /// <summary>
  /// TEst if group name is reserved
  /// </summary>
  /// <param name="groupName">Group name to test</param>
  /// <returns></returns>
  public static bool IsReserved(string groupName)
  {
    return ReservedNames.Contains(groupName);
  }

}
