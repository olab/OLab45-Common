using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table("map_node_priorities")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapNodePriorities
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
