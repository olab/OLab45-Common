using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_nodes_copy")]
[Index("LinkStyleId", Name = "link_style_id")]
[Index("MapId", Name = "map_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapNodesCopy
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("map_id")]
    public uint MapId { get; set; }

    [Column("title")]
    [StringLength(200)]
    public string Title { get; set; }

    [Column("text", TypeName = "text")]
    public string Text { get; set; }

    [Column("type_id")]
    public uint? TypeId { get; set; }

    [Column("probability", TypeName = "tinyint(1)")]
    public sbyte? Probability { get; set; }

    [Column("conditional")]
    [StringLength(500)]
    public string Conditional { get; set; }

    [Column("conditional_message")]
    [StringLength(1000)]
    public string ConditionalMessage { get; set; }

    [Column("info", TypeName = "text")]
    public string Info { get; set; }

    [Column("is_private")]
    public int IsPrivate { get; set; }

    [Column("link_style_id")]
    public uint? LinkStyleId { get; set; }

    [Column("link_type_id")]
    public uint? LinkTypeId { get; set; }

    [Column("priority_id")]
    public int? PriorityId { get; set; }

    [Column("kfp", TypeName = "tinyint(1)")]
    public sbyte? Kfp { get; set; }

    [Column("undo", TypeName = "tinyint(1)")]
    public sbyte? Undo { get; set; }

    [Column("end", TypeName = "tinyint(1)")]
    public sbyte? End { get; set; }

    [Column("x")]
    public double? X { get; set; }

    [Column("y")]
    public double? Y { get; set; }

    [Column("rgb")]
    [StringLength(8)]
    public string Rgb { get; set; }

    [Column("show_info")]
    public sbyte ShowInfo { get; set; }

    [Column("annotation", TypeName = "text")]
    public string Annotation { get; set; }

    [Column("height")]
    public int? Height { get; set; }

    [Column("width")]
    public int? Width { get; set; }

    [Column("locked")]
    public int? Locked { get; set; }

    [Column("collapsed")]
    public int? Collapsed { get; set; }

    [Column("visit_once")]
    public int? VisitOnce { get; set; }

    [Column("force_reload")]
    public int? ForceReload { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_At", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("LinkStyleId")]
    [InverseProperty("MapNodesCopy")]
    public virtual MapNodeLinkStylies LinkStyle { get; set; }

    [ForeignKey("MapId")]
    [InverseProperty("MapNodesCopy")]
    public virtual Maps Map { get; set; }
}
