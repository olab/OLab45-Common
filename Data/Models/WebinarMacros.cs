using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Data.Models
{
  [Table("webinar_macros")]
  [Index(nameof(WebinarId), Name = "webinar_id")]
  public partial class WebinarMacros
  {
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }
    [Column("text")]
    [StringLength(255)]
    public string Text { get; set; }
    [Column("hot_keys")]
    [StringLength(255)]
    public string HotKeys { get; set; }
    [Column("webinar_id", TypeName = "int(10) unsigned")]
    public uint WebinarId { get; set; }
  }
}
