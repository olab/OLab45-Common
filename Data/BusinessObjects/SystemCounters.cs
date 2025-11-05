using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("system_counters")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class SystemCounters
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; }

    [Column("description", TypeName = "text")]
    public string Description { get; set; }

    [Column("start_value", TypeName = "blob")]
    public byte[] StartValue { get; set; }

    [Column("value", TypeName = "blob")]
    public byte[] Value { get; set; }

    [Column("icon_id")]
    public int? IconId { get; set; }

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

    [Column("imageable_id")]
    public uint ImageableId { get; set; }

    [Required]
    [Column("imageable_type")]
    [StringLength(45)]
    public string ImageableType { get; set; }

    [Column("is_system")]
    public int? IsSystem { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_At", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Counter")]
    public virtual ICollection<SystemCounterActions> SystemCounterActions { get; set; } = new List<SystemCounterActions>();
}
