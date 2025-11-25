using DocumentFormat.OpenXml.Vml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

public partial class MapNodeLinks
{
  [NotMapped]
  public bool? IsHidden
  {
    get => Hidden.HasValue ? Hidden.Value == 1 : (bool?)null;
    set => Hidden = value.HasValue ? (value.Value ? (sbyte)1 : (sbyte)0) : (sbyte?)null;
  }

  public static MapNodeLinks CreateDefault()
  {
    return new MapNodeLinks
    {
      IsHidden = false,
      Order = 0,
      Probability = 0,
      LinkStyleId = 5,
      ImageId = 0,
      LineType = 1,
      Thickness = 2,
      Color = "#566e94",
      FollowOnce = 0,
      Text = ""
    };
  }

  public static void Reassign(Dictionary<uint, uint> reverseNodeIdMap, uint mapId, MapNodeLinks link)
  {
    link.Id = 0;
    link.MapId = mapId;

    if ( reverseNodeIdMap.ContainsKey( link.NodeId1 ) )
      link.NodeId1 = reverseNodeIdMap[ link.NodeId1 ];
    else
      throw new Exception( $"NodeId1 {link.NodeId1} not found in mapping" );

    if ( reverseNodeIdMap.ContainsKey( link.NodeId2 ) )
      link.NodeId2 = reverseNodeIdMap[ link.NodeId2 ];
    else
      throw new Exception( $"NodeId2 {link.NodeId1} not found in mapping" );

  }

}
