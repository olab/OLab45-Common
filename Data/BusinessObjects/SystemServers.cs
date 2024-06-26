using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Api.Model;

[Table("system_servers")]
public partial class SystemServers
{
  [Key]
  [Column("id", TypeName = "int(10) unsigned")]
  public uint Id { get; set; }
  [Required]
  [Column("name")]
  [StringLength(200)]
  public string Name { get; set; }
  [Column("description")]
  [StringLength(200)]
  public string Description { get; set; }
  [Column("created_at", TypeName = "datetime")]
  public DateTime? CreatedAt { get; set; }
  [Column("updated_At", TypeName = "datetime")]
  public DateTime? UpdatedAt { get; set; }
}
