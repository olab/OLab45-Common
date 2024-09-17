using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table("map_node_links")]
[Index("MapId", Name = "map_id")]
[Index("NodeId1", Name = "node_id_1")]
[Index("NodeId2", Name = "node_id_2")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapNodeLinks
{
  [Key]
  [Column("id", TypeName = "int(10) unsigned")]
  public uint Id { get; set; }

  [Column("map_id", TypeName = "int(10) unsigned")]
  public uint MapId { get; set; }

  [Column("node_id_1", TypeName = "int(10) unsigned")]
  public uint NodeId1 { get; set; }

  [Column("node_id_2", TypeName = "int(10) unsigned")]
  public uint NodeId2 { get; set; }

  [Column("image_id", TypeName = "int(10) unsigned")]
  public uint? ImageId { get; set; }

  [Column("text")]
  [StringLength(500)]
  public string Text { get; set; }

  [Column("order", TypeName = "int(10)")]
  public int? Order { get; set; }

  [Column("probability", TypeName = "int(10)")]
  public int? Probability { get; set; }

  [Column("hidden")]
  public bool? Hidden { get; set; }

  [Column("link_style_id", TypeName = "int(10) unsigned")]
  public uint? LinkStyleId { get; set; }

  [Column("thickness", TypeName = "int(10)")]
  public int? Thickness { get; set; }

  [Column("line_type", TypeName = "int(10)")]
  public int? LineType { get; set; }

  [Column("color")]
  [StringLength(45)]
  public string Color { get; set; }

  [Column("follow_once", TypeName = "int(4)")]
  public int? FollowOnce { get; set; }

  [Column("created_at", TypeName = "datetime")]
  public DateTime? CreatedAt { get; set; }

  [Column("updated_At", TypeName = "datetime")]
  public DateTime? UpdatedAt { get; set; }

  [ForeignKey("MapId")]
  [InverseProperty("MapNodeLinks")]
  public virtual Maps Map { get; set; }

  [ForeignKey("NodeId1")]
  [InverseProperty("MapNodeLinksNodeId1Navigation")]
  public virtual MapNodes NodeId1Navigation { get; set; }

  [ForeignKey("NodeId2")]
  [InverseProperty("MapNodeLinksNodeId2Navigation")]
  public virtual MapNodes NodeId2Navigation { get; set; }
}
