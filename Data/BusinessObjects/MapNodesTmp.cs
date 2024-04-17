using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Keyless]
[Table("map_nodes_tmp")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapNodesTmp
{
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Column("map_id", TypeName = "int(10) unsigned")]
    public uint MapId { get; set; }

    [Column("title")]
    [StringLength(200)]
    public string Title { get; set; }

    [Column("text", TypeName = "text")]
    public string Text { get; set; }

    [Column("type_id", TypeName = "int(10) unsigned")]
    public uint? TypeId { get; set; }

    [Column("probability")]
    public bool? Probability { get; set; }

    [Column("conditional")]
    [StringLength(500)]
    public string Conditional { get; set; }

    [Column("conditional_message")]
    [StringLength(1000)]
    public string ConditionalMessage { get; set; }

    [Column("info", TypeName = "text")]
    public string Info { get; set; }

    [Column("is_private", TypeName = "int(4)")]
    public int IsPrivate { get; set; }

    [Column("link_style_id", TypeName = "int(10) unsigned")]
    public uint? LinkStyleId { get; set; }

    [Column("link_type_id", TypeName = "int(10) unsigned")]
    public uint? LinkTypeId { get; set; }

    [Column("priority_id", TypeName = "int(10)")]
    public int? PriorityId { get; set; }

    [Column("kfp")]
    public bool? Kfp { get; set; }

    [Column("undo")]
    public bool? Undo { get; set; }

    [Column("end")]
    public bool? End { get; set; }

    [Column("x")]
    public double? X { get; set; }

    [Column("y")]
    public double? Y { get; set; }

    [Column("rgb")]
    [StringLength(8)]
    public string Rgb { get; set; }

    [Column("show_info", TypeName = "tinyint(4)")]
    public sbyte ShowInfo { get; set; }

    [Column("annotation", TypeName = "text")]
    public string Annotation { get; set; }

    [Column("height", TypeName = "int(10)")]
    public int? Height { get; set; }

    [Column("width", TypeName = "int(10)")]
    public int? Width { get; set; }

    [Column("locked", TypeName = "int(10)")]
    public int? Locked { get; set; }

    [Column("collapsed", TypeName = "int(10)")]
    public int? Collapsed { get; set; }

    [Column("visit_once", TypeName = "int(4)")]
    public int? VisitOnce { get; set; }

    [Column("force_reload", TypeName = "int(10)")]
    public int? ForceReload { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_At", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }
}
