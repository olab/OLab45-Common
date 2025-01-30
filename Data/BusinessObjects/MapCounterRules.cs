using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table( "map_counter_rules" )]
[Index( "CounterId", Name = "counter_id" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class MapCounterRules
{
  [Key]
  [Column( "id", TypeName = "int(10) unsigned" )]
  public uint Id { get; set; }

  [Column( "counter_id", TypeName = "int(10) unsigned" )]
  public uint CounterId { get; set; }

  [Column( "relation_id", TypeName = "int(10) unsigned" )]
  public uint RelationId { get; set; }

  [Column( "value" )]
  public double Value { get; set; }

  [Column( "function" )]
  [StringLength( 50 )]
  public string Function { get; set; }

  [Column( "redirect_node_id", TypeName = "int(10)" )]
  public int? RedirectNodeId { get; set; }

  [Column( "message" )]
  [StringLength( 500 )]
  public string Message { get; set; }

  [Column( "counter", TypeName = "int(10)" )]
  public int? Counter { get; set; }

  [Column( "counter_value" )]
  [StringLength( 50 )]
  public string CounterValue { get; set; }

  [ForeignKey( "CounterId" )]
  [InverseProperty( "MapCounterRules" )]
  public virtual MapCounters CounterNavigation { get; set; }
}
