using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Data.Models;

[Table("map_node_types")]
public partial class MapNodeTypes
{
  [Key]
  [Column("id", TypeName = "int(10) unsigned")]
  public uint Id { get; set; }
  [Required]
  [Column("name")]
  [StringLength(70)]
  public string Name { get; set; }
  [Required]
  [Column("description")]
  [StringLength(500)]
  public string Description { get; set; }
}
