using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_node_jumps")]
[Index("MapId", Name = "map_id")]
[Index("NodeId", Name = "node_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapNodeJumps
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("map_id")]
    public uint MapId { get; set; }

    [Column("node_id")]
    public uint NodeId { get; set; }

    [Column("image_id")]
    public uint? ImageId { get; set; }

    [Column("text")]
    [StringLength(500)]
    public string Text { get; set; }

    [Column("order")]
    public int? Order { get; set; }

    [Column("probability")]
    public int? Probability { get; set; }

    [Column("hidden")]
    public bool? Hidden { get; set; }

    [Column("link_style_id")]
    public uint? LinkStyleId { get; set; }

    [Column("thickness")]
    public int? Thickness { get; set; }

    [Column("line_type")]
    public int? LineType { get; set; }

    [Column("color")]
    [StringLength(45)]
    public string Color { get; set; }

    [Column("follow_once")]
    public int? FollowOnce { get; set; }

    [ForeignKey("MapId")]
    [InverseProperty("MapNodeJumps")]
    public virtual Maps Map { get; set; }

    [ForeignKey("NodeId")]
    [InverseProperty("MapNodeJumps")]
    public virtual MapNodes Node { get; set; }
}
