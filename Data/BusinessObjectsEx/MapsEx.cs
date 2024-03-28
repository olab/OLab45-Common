using AutoMapper;
using Dawn;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

#nullable disable

namespace OLab.Api.Model;

public partial class Maps
{

  /// <summary>
  /// Create default map given a template map
  /// </summary>
  /// <param name="user">Owning user</param>
  /// <param name="templateMap"></param>
  /// <returns></returns>
  public static Maps CreateDefault(Maps templateMap)
  {
    Guard.Argument(templateMap).NotNull(nameof(templateMap));

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

  /// <summary>
  /// Add users groups to map
  /// </summary>
  /// <param name="user">Source user</param>
  public void AddGroupsFromUser(Users user)
  {
    // Add the users groups to the map
    if (user.UserGroups.Count > 0)
    {
      foreach (var userUserGroup in user.UserGroups)
      {
        var mapGroup = new MapGroups { MapId = Id, GroupId = userUserGroup.GroupId };
        MapGroups.Add(mapGroup);
      }
    }
  }

  /// <summary>
  /// Create default map given owning user
  /// </summary>
  /// <param name="user">Owning user</param>
  /// <returns>Map</returns>
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
      ReportNodeId = 0
    };

    map.MapNodes.Add(new MapNodes { Title = "New Node", TypeId = 1, Text = "Sample Text" });

    return map;
  }

  /// <summary>
  /// Test if map can be anonymously invoked
  /// </summary>
  /// <returns>true/false</returns>
  public bool IsAnonymous()
  {
    return SecurityId == 1;
  }
}
