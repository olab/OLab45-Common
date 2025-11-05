using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_counters")]
[Index("MapId", Name = "map_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapCounters
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("map_id")]
    public uint? MapId { get; set; }

    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; }

    [Column("description", TypeName = "text")]
    public string Description { get; set; }

    [Column("start_value")]
    public double StartValue { get; set; }

    [Column("icon_id")]
    public uint? IconId { get; set; }

    [Column("prefix")]
    [StringLength(20)]
    public string Prefix { get; set; }

    [Column("suffix")]
    [StringLength(20)]
    public string Suffix { get; set; }

    [Column("visible")]
    public bool? Visible { get; set; }

    [Column("out_of")]
    public int? OutOf { get; set; }

    [Column("status")]
    public int Status { get; set; }

    [ForeignKey("MapId")]
    [InverseProperty("MapCounters")]
    public virtual Maps Map { get; set; }

    [InverseProperty("CounterNavigation")]
    public virtual ICollection<MapCounterRules> MapCounterRules { get; set; } = new List<MapCounterRules>();
}
