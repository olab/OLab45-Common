using OLab.Api.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Data.Models;

[Table("map_sections")]
public partial class MapSections
{
  public MapSections()
  {
    Maps = new HashSet<Maps>();
  }

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

  [InverseProperty("Section")]
  public virtual ICollection<Maps> Maps { get; set; }
}
