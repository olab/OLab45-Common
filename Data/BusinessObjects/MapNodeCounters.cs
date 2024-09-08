using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_node_counters")]
[Index("NodeId", Name = "node_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapNodeCounters
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Column("node_id", TypeName = "int(10) unsigned")]
    public uint NodeId { get; set; }

    [Column("counter_id", TypeName = "int(10) unsigned")]
    public uint CounterId { get; set; }

    [Required]
    [Column("function")]
    [StringLength(20)]
    public string Function { get; set; }

    [Column("display", TypeName = "int(10)")]
    public int Display { get; set; }

    [ForeignKey("NodeId")]
    [InverseProperty("MapNodeCounters")]
    public virtual MapNodes Node { get; set; }
}
