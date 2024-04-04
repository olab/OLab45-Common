using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Data.Interface;
using System.Collections.Generic;
using System.Linq;

#nullable disable

namespace OLab.Api.Model;

public partial class Maps
{
  public bool IsAnonymous()
  {
    return SecurityId != 1;
  }

  public static Maps CreateDefault(Maps templateMap)
  {
    // use AutoMapper to do a Deep Copy
    var mapper = new Mapper(new MapperConfiguration(cfg =>
      cfg.CreateMap<Maps, Maps>().ReverseMap()
    ));

    var map = mapper.Map<Maps>(templateMap);

    map.Id = 0;
    map.IsTemplate = 0;

    return map;
  }

  public void AppendMapNodes(Maps sourceMap)
  {
    foreach (var node in sourceMap.MapNodes)
      MapNodes.Add(node);
  }

  public static Maps CreateDefault()
  {
    var map = new Maps
    {
      Abstract = "<b>New Map</b>",
      DeltaTime = 0,
      DevNotes = "",
      Enabled = true,
      Feedback = "",
      Guid = "",
      IsTemplate = 0,
      Keywords = "",
      LanguageId = 1,
      Name = "New Map",
      ReminderMsg = "",
      ReminderTime = 0,
      RendererVersion = 4,
      SectionId = 2,
      SecurityId = 3,
      SendXapiStatements = false,
      ShowBar = false,
      ShowScore = false,
      SkinId = 1,
      SourceId = 0,
      Source = "",
      TypeId = 11,
      Units = "",
      Verification = "{}",
      // MapNodes = new List<MapNodes>(),
      ReportNodeId = 0
    };

    map.MapNodes.Add(new MapNodes { Title = "New Node", TypeId = 1, Text = "Sample Text" });

    return map;
  }

  /// <summary>
  /// Assign user's authorization configuration to map
  /// </summary>
  /// <param name="dbContext">OLabDBContext</param>
  /// <param name="userContext">User context</param>
  public void AssignAuthorization(OLabDBContext dbContext, IUserContext userContext)
  {
    AuthorId = userContext.UserId;

    AssignMapGroups(dbContext, userContext);
    AssignSecurityRoles(dbContext, userContext);
    AssignSecurityUsers(dbContext);
  }

  private void AssignSecurityUsers(OLabDBContext dbContext)
  {
    dbContext.SecurityUsers.Add(new SecurityUsers
    {
      Acl = "RWXD",
      ImageableId = Id,
      ImageableType = "Maps",
      Iss = "olab",
      UserId = AuthorId
    });
  }

  private void AssignMapGroups(OLabDBContext dbContext, IUserContext userContext)
  {
    foreach (var item in userContext.UserRoles)
    {
      // user must belong to an explicit 'Importer' role
      // on a group in order for the group to be added.

      if ((item.Role.Name != Roles.RoleNameImporter) || (item.Role.Name != Roles.RoleNameSuperuser))
        continue;

      MapGroups.Add(Model.MapGroups.FromGroupNames(
        dbContext,
        item.Group.Name));
    }
  }

  private void AssignSecurityRoles(OLabDBContext dbContext, IUserContext userContext)
  {
    var roleLearnerPhys = dbContext.Roles.FirstOrDefault(x => x.Name == Roles.RoleNameLearner);
    var roleAuthorPhys = dbContext.Roles.FirstOrDefault(x => x.Name == Roles.RoleNameAuthor);

    foreach (var item in userContext.UserRoles)
    {
      // user must belong to an explicit 'Importer' role
      // on a group in order for the security roles to be added.

      if ((item.Role.Name != Roles.RoleNameImporter) || (item.Role.Name != Roles.RoleNameSuperuser))
        continue;

      dbContext.SecurityRoles.Add(new SecurityRoles
      {
        Acl = "RX",
        ImageableId = Id,
        ImageableType = "Maps",
        GroupId = item.GroupId,
        RoleId = roleLearnerPhys.Id
      });

      dbContext.SecurityRoles.Add(new SecurityRoles
      {
        Acl = "RWXD",
        ImageableId = Id,
        ImageableType = "Maps",
        GroupId = item.GroupId,
        RoleId = roleAuthorPhys.Id
      });
    }
  }
}
