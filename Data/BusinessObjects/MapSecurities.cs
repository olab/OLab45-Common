using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Api.Model;

[Table("map_securities")]
public partial class MapSecurities
{
  public MapSecurities()
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

  [InverseProperty("Security")]
  public virtual ICollection<Maps> Maps { get; set; }
}
