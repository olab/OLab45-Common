using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table("map_types")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapTypes
{
  [Key]
  [Column("id", TypeName = "int(10) unsigned")]
  public uint Id { get; set; }

  [Required]
  [Column("name")]
  [StringLength(100)]
  public string Name { get; set; }

  [Required]
  [Column("description")]
  [StringLength(700)]
  public string Description { get; set; }

  [InverseProperty("Type")]
  public virtual ICollection<Maps> Maps { get; } = new List<Maps>();
}
