using Microsoft.EntityFrameworkCore;
using OLab.Api.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Data.Models;

[Table("map_node_counters")]
[Index(nameof(NodeId), Name = "node_id")]
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

  [ForeignKey(nameof(NodeId))]
  [InverseProperty(nameof(MapNodes.MapNodeCounters))]
  public virtual MapNodes Node { get; set; }
}
