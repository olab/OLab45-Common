using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_counter_common_rules")]
[Index("MapId", Name = "map_id")]
public partial class MapCounterCommonRules
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Column("map_id", TypeName = "int(10) unsigned")]
    public uint MapId { get; set; }

    [Required]
    [Column("rule")]
    public string Rule { get; set; }

    [Column("lightning", TypeName = "int(10)")]
    public int Lightning { get; set; }

    [Column("isCorrect")]
    public bool IsCorrect { get; set; }

    [InverseProperty("Rule")]
    public virtual ICollection<Cron> Cron { get; } = new List<Cron>();

    [ForeignKey("MapId")]
    [InverseProperty("MapCounterCommonRules")]
    public virtual Maps Map { get; set; }
}
