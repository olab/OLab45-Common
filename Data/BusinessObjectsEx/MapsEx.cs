using AutoMapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using OLab.Common.Utils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

#nullable disable

namespace OLab.Api.Model;

public partial class Maps
{

  [NotMapped]
  public bool IsEnabled
  {
    get => Enabled == 1;
    set => Enabled = value ? (sbyte)1 : (sbyte)0;
  }

  [NotMapped]
  public bool IsSendXapiStatements
  {
    get => SendXapiStatements == 1;
    set => SendXapiStatements = value ? (sbyte)1 : (sbyte)0;
  }

  [NotMapped]
  public bool IsShowBar
  {
    get => ShowBar == 1;
    set => ShowBar = value ? (sbyte)1 : (sbyte)0;
  }

  [NotMapped]
  public bool IsShowScore
  {
    get => ShowScore == 1;
    set => ShowScore = value ? (sbyte)1 : (sbyte)0;
  }

  public const int MapSecurityAnonymous = 1;

  public static Maps CreateDefault(Maps templateMap)
  {
    // use AutoMapper to do a Deep Copy
    var mapper = new Mapper( new MapperConfiguration( cfg =>
      cfg.CreateMap<Maps, Maps>().ReverseMap()
    ) );

    var map = mapper.Map<Maps>( templateMap );

    map.Id = 0;
    map.IsTemplate = 0;

    return map;
  }

  public void AppendMapNodes(Maps sourceMap)
  {
    foreach ( var node in sourceMap.MapNodes )
      MapNodes.Add( node );
  }

  public static Maps CreateDefault()
  {
    var map = new Maps
    {
      Abstract = "<b>New Map</b>",
      DeltaTime = 0,
      DevNotes = "",
      IsEnabled = true,
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
      IsSendXapiStatements = false,
      IsShowBar = false,
      IsShowScore = false,
      SkinId = 1,
      SourceId = 0,
      Source = "",
      TypeId = 11,
      Units = "",
      Verification = "{}",
      ReportNodeId = 0
    };

    map.MapNodes.Add( new MapNodes { Title = "New Node", TypeId = 1, Text = "Sample Text" } );

    return map;
  }

  public static string TruncateToJsonObject(IList<Maps> physMaps, int maxDepth)
  {
    var json = JsonConvert.SerializeObject(
      physMaps,
      new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore } );

    return SerializerUtilities.TruncateJsonToDepth( json, maxDepth + 1 );
  }

  /// <summary>
  /// Tests if a map is accessible purely on group/role
  /// </summary>
  /// <param name="phys"></param>
  /// <param name="groupId"></param>
  /// <param name="roleId"></param>
  /// <returns></returns>
  public static bool IsAccessible(Maps phys, uint? groupId, uint? roleId)
  {
    var accessible = (phys.MapGrouproles.Any( y => (y.GroupId == groupId && y.RoleId == roleId) ) ||
        phys.MapGrouproles.Any( y => (y.GroupId == groupId && !y.RoleId.HasValue) ) ||
        phys.MapGrouproles.Any( y => (!y.GroupId.HasValue && y.RoleId == roleId) ) ||
        phys.MapGrouproles.Any( y => (!y.GroupId.HasValue && !y.RoleId.HasValue) ));

    return accessible;
  }

  public override string ToString()
  {
    return $"{Name} ({Id})";
  }

}
