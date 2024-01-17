using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Data.Models;

[Table("map_popups_counters")]
[Index(nameof(CounterId), Name = "counter_id")]
[Index(nameof(PopupId), Name = "popup_id")]
public partial class MapPopupsCounters
{
  [Key]
  [Column("id", TypeName = "int(10) unsigned")]
  public uint Id { get; set; }
  [Column("popup_id", TypeName = "int(10) unsigned")]
  public uint PopupId { get; set; }
  [Column("counter_id", TypeName = "int(10) unsigned")]
  public uint CounterId { get; set; }
  [Required]
  [Column("function", TypeName = "text")]
  public string Function { get; set; }

  [ForeignKey(nameof(PopupId))]
  [InverseProperty(nameof(MapPopups.MapPopupsCounters))]
  public virtual MapPopups Popup { get; set; }
}
