using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Api.Model;

public partial class MapNodes
{
  [NotMapped]
  public bool? IsProbability
  {
    get => Probability.HasValue ? Probability.Value == 1 : (bool?)null;
    set => Probability = value.HasValue ? (value.Value ? (sbyte)1 : (sbyte)0) : (sbyte?)null;
  }

  [NotMapped]
  public bool? IsEnd
  {
    get => End.HasValue ? End.Value == 1 : (bool?)null;
    set => End = value.HasValue ? (value.Value ? (sbyte)1 : (sbyte)0) : (sbyte?)null;
  }

  [NotMapped]
  public bool? IsKfp
  {
    get => Kfp.HasValue ? Kfp.Value == 1 : (bool?)null;
    set => Kfp = value.HasValue ? (value.Value ? (sbyte)1 : (sbyte)0) : (sbyte?)null;
  }

  [NotMapped]
  public bool? IsUndo
  {
    get => Undo.HasValue ? Undo.Value == 1 : (bool?)null;
    set => Undo = value.HasValue ? (value.Value ? (sbyte)1 : (sbyte)0) : (sbyte?)null;
  }

  [NotMapped]
  public bool IsShowInfo
  {
    get => ShowInfo == 1;
    set => ShowInfo = value ? (sbyte)1 : (sbyte)0;
  }

  public enum NodeType : int
  {
    RootNode = 1,
    ContentNode = 2
  }

  public static MapNodes CreateDefault()
  {
    return new MapNodes
    {
      Title = "New Node",
      Text = "New node text",
      X = 0,
      Y = 0,
      Locked = 0,
      Collapsed = 0,
      Height = 160,
      Width = 300,
      IsProbability = false,
      Info = "",
      Annotation = "",
      LinkStyleId = 5,
      LinkTypeId = 1,
      TypeId = 2,
      IsPrivate = 0,
      VisitOnce = 0,
      PriorityId = 1,
      IsEnd = false,
      Rgb = "#566e94",
      CreatedAt = DateTime.UtcNow,
    };
  }

  public static bool Reassign(Maps destinationMap, MapNodes node)
  {
    // test if node has already been re-assigned
    if ( node.Id == 0 )
      return false;

    node.Id = 0;
    node.MapId = destinationMap.Id;

    return true;
  }

}
