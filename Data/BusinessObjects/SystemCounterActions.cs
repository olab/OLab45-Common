﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("system_counter_actions")]
[Index("CounterId", Name = "fk_counter_action_counter_idx")]
[Index("MapId", Name = "fk_counter_action_map")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class SystemCounterActions
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Column("counter_id", TypeName = "int(10) unsigned")]
    public uint CounterId { get; set; }

    [Column("map_id", TypeName = "int(10) unsigned")]
    public uint? MapId { get; set; }

    [Required]
    [Column("operation_type")]
    [StringLength(45)]
    public string OperationType { get; set; }

    [Required]
    [Column("expression")]
    [StringLength(256)]
    public string Expression { get; set; }

    [Column("visible", TypeName = "int(10)")]
    public int Visible { get; set; }

    [Column("imageable_id", TypeName = "int(10) unsigned")]
    public uint ImageableId { get; set; }

    [Required]
    [Column("imageable_type")]
    [StringLength(45)]
    public string ImageableType { get; set; }

    [ForeignKey("CounterId")]
    [InverseProperty("SystemCounterActions")]
    public virtual SystemCounters Counter { get; set; }

    [ForeignKey("MapId")]
    [InverseProperty("SystemCounterActions")]
    public virtual Maps Map { get; set; }
}
