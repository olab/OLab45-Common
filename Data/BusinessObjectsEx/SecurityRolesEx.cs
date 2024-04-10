#nullable disable

using DocumentFormat.OpenXml.Vml;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace OLab.Api.Model;

public partial class SecurityRoles
{
  public const uint Read = 0b100;
  public const uint Write = 0b010;
  public const uint Execute = 0b001;
  public const uint NoAccess = 0b000;
  public const uint AllAccess = 0b111;

  public static IList<SecurityRoles> GetAcls(OLabDBContext dbContext, UserGroups userGroup)
  {
    var securityRoles = dbContext.SecurityRoles
      .Where(x => x.GroupId == userGroup.GroupId && x.RoleId == userGroup.RoleId).ToList();

    return securityRoles;
  }

  public static IList<SecurityRoles> CreateDefaultForGroup(
    OLabDBContext dbContext, 
    uint groupId, 
    string scopeLevelType, 
    uint scopeObjectId)
  {
    var roles = new List<SecurityRoles>();

    var groupPhys = dbContext.Groups.FirstOrDefault(x => x.Id == groupId);
    if (groupPhys == null)
      throw new OLabObjectNotFoundException("Groups", groupId);

    var rolePhys = dbContext.Roles
      .FirstOrDefault(x => x.Name == Roles.RoleNameSuperuser);

    if (rolePhys == null)
      throw new OLabObjectNotFoundException("Roles", Roles.RoleNameSuperuser);
    roles.Add(new SecurityRoles { 
      GroupId = groupId, 
      RoleId = rolePhys.Id, 
      Acl2 = AllAccess,
      ImageableType = scopeLevelType,
      ImageableId = scopeObjectId});

    rolePhys = dbContext.Roles
      .FirstOrDefault(x => x.Name == Roles.RoleNameLearner);

    if (rolePhys == null)
      throw new OLabObjectNotFoundException("Roles", Roles.RoleNameLearner);
    roles.Add(new SecurityRoles { 
      GroupId = groupId, 
      RoleId = rolePhys.Id, 
      Acl2 = Read | Execute,
      ImageableType = scopeLevelType,
      ImageableId = scopeObjectId});

    rolePhys = dbContext.Roles
      .FirstOrDefault(x => x.Name == Roles.RoleNameImporter);

    if (rolePhys == null)
      throw new OLabObjectNotFoundException("Roles", Roles.RoleNameImporter);
    roles.Add(new SecurityRoles { 
      GroupId = groupId, 
      RoleId = rolePhys.Id, 
      Acl2 = AllAccess,
      ImageableType = scopeLevelType,
      ImageableId = scopeObjectId});

    rolePhys = dbContext.Roles
      .FirstOrDefault(x => x.Name == Roles.RoleNameModerator);

    if (rolePhys == null)
      throw new OLabObjectNotFoundException("Roles", Roles.RoleNameModerator);
    roles.Add(new SecurityRoles { 
      GroupId = groupId, 
      RoleId = rolePhys.Id, 
      Acl2 = Read | Execute,
      ImageableType = scopeLevelType,
      ImageableId = scopeObjectId});

    rolePhys = dbContext.Roles
      .FirstOrDefault(x => x.Name == Roles.RoleNameAuthor);

    if (rolePhys == null)
      throw new OLabObjectNotFoundException("Roles", Roles.RoleNameAuthor);
    roles.Add(new SecurityRoles { 
      GroupId = groupId, 
      RoleId = rolePhys.Id, 
      Acl2 = AllAccess,
      ImageableType = scopeLevelType,
      ImageableId = scopeObjectId});

    return roles;
  }

  public override string ToString()
  {
    return $"{Id}: {GroupId} {RoleId} {ImageableType}({ImageableId}) '{Acl} {Acl2}'";
  }

}
