using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_counter_common_rules")]
[Index("MapId", Name = "map_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapCounterCommonRules
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("map_id")]
    public uint MapId { get; set; }

    [Required]
    [Column("rule")]
    public string Rule { get; set; }

    [Column("lightning")]
    public int Lightning { get; set; }

    [Column("correct", TypeName = "tinyint(1)")]
    public sbyte Correct { get; set; }

    [InverseProperty("Rule")]
    public virtual ICollection<Cron> Cron { get; set; } = new List<Cron>();

    [ForeignKey("MapId")]
    [InverseProperty("MapCounterCommonRules")]
    public virtual Maps Map { get; set; }
}
