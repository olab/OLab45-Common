using Newtonsoft.Json;
using OLab.Common.Interfaces;
using OLab.Data.ReaderWriters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace OLab.Api.Model;

//public class GroupRole
//{
//  public uint GroupId { get; set; }
//  public uint RoleId { get; set; }
//}

public class AddUserRequest
{
  private IOLabLogger logger;
  private OLabDBContext dbContext;

  public uint? Id { get; set; }

  [Required]
  public string Username { get; set; }
  [Required]
  public string EMail { get; set; }
  [Required]
  public string NickName { get; set; }
  public string Password { get; set; }
  public string ModeUi { get; set; }
  public string GroupRoles { get; set; }
  public string Operation {  get; set; }

  public IList<UserGrouproles> GroupRoleObjects { get; } = new List<UserGrouproles>();


  public AddUserRequest()
  {
    NickName = "";
    ModeUi = "easy";
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
    OLabDBContext dbContext)
  {
    this.logger = logger;
    this.dbContext = dbContext;
  }

  public void SetInfrastructure(
    IOLabLogger logger,
    OLabDBContext dbContext)
  {
    this.logger = logger;
    this.dbContext = dbContext;
  }

  public void BuildGroupRoleObjects(string groupRoleString = null)
  {
    // if not passed in, then look for it in the GroupRoles string
    // which is what the EditUser method may pass in
    if (groupRoleString == null)
      groupRoleString = GroupRoles;

    var groupRoleParts = groupRoleString.Split(",");
    foreach (var groupRolePart in groupRoleParts)
    {
      if (string.IsNullOrEmpty(groupRolePart))
        continue;

      var obj = UserGrouproles.StringToObject(dbContext, groupRolePart);
      if (obj == null)
        continue;

      if (Id.HasValue)
        obj.UserId = Id.Value;

      if (obj != null)
        GroupRoleObjects.Add(obj);
    }
  }

  public void ProcessAddUserText(string userRequestText)
  {
    var userRequestParts = userRequestText.Split("\t");
    if (userRequestParts.Length < 5)
      throw new Exception("Bad user request record");

    Username = userRequestParts[0];
    if (userRequestParts[1].Length > 0)
      Password = userRequestParts[1];

    EMail = userRequestParts[2];
    NickName = userRequestParts[3];

    // process [ group:role,... ] strings
    for (var i = 4; i < userRequestParts.Length; i++)
      BuildGroupRoleObjects(userRequestParts[i]);

    Username = Username.ToLower();

  }

}