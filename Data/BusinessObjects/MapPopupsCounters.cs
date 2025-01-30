using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table( "map_popups_counters" )]
[Index( "CounterId", Name = "counter_id" )]
[Index( "PopupId", Name = "popup_id" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class MapPopupsCounters
{
  [Key]
  [Column( "id", TypeName = "int(10) unsigned" )]
  public uint Id { get; set; }

  [Column( "popup_id", TypeName = "int(10) unsigned" )]
  public uint PopupId { get; set; }

  [Column( "counter_id", TypeName = "int(10) unsigned" )]
  public uint CounterId { get; set; }

  [Required]
  [Column( "function", TypeName = "text" )]
  public string Function { get; set; }

  [ForeignKey( "PopupId" )]
  [InverseProperty( "MapPopupsCounters" )]
  public virtual MapPopups Popup { get; set; }
}
