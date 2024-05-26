using Microsoft.Build.Framework;
using Newtonsoft.Json;
using OLab.Common.Interfaces;
using OLab.Data.ReaderWriters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace OLab.Api.Model;

//public class GroupRole
//{
//  public uint GroupId { get; set; }
//  public uint RoleId { get; set; }
//}

public class AddUserRequest
{
  //private readonly string userRequestText;

  [Required]
  public string Username { get; set; }
  [Required]
  public string EMail { get; set; }
  [Required]
  public string NickName { get; set; }
  public string Password { get; set; }
  public string ModeUi { get; set; }

  public IList<UserGrouproles> GroupRoles { get; set; }

  //[Required]
  //public string Group { get; set; }
  //[Required]
  //public string Role { get; set; }

  public AddUserRequest()
  {
    NickName = "";
    ModeUi = "easy";
    GroupRoles = new List<UserGrouproles>();
  }

  /// <summary>
  /// Load user request from controller string
  /// </summary>
  /// <param name="logger">IOLabLogger</param>
  /// <param name="dbContext">OLabDbContext</param>
  /// <param name="userRequestText">Controller text</param>
  /// <exception cref="Exception"></exception>
  public AddUserRequest(
    IOLabLogger logger,
    OLabDBContext dbContext,
    string userRequestText)
  {
    var parts = userRequestText.Split("\t");
    if (parts.Length < 5)
    {
      throw new Exception("Bad user request record");
    }

    Username = parts[0];
    if (parts[1].Length > 0)
      Password = parts[1];

    EMail = parts[2];
    NickName = parts[3];

    if (parts.Count() <= 4)
      throw new Exception("Missing group/role arguments");

    // process group.role strings
    for (int i = 4; i < parts.Length; i++)
    {
      var groupRolePart = parts[i];
      var groupRoleParts = groupRolePart.Split(":");

      var reader = GroupRoleReaderWriter.Instance(logger, dbContext);
      var groupPhys = reader.GetGroup(groupRoleParts[0]);
      var rolesPhys = reader.GetRole(groupRoleParts[1]);

      if ( (groupPhys != null) && (rolesPhys != null) )
        GroupRoles.Add(new UserGrouproles 
        { 
          GroupId = groupPhys.Id, 
          RoleId = rolesPhys.Id,
          Group = groupPhys,
          Role = rolesPhys
        });
    }

    Username = Username.ToLower();

  }

}