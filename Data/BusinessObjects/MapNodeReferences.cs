using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table("map_node_references")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapNodeReferences
{
  [Key]
  [Column("id", TypeName = "int(10) unsigned")]
  public uint Id { get; set; }

  [Column("map_id", TypeName = "int(10) unsigned")]
  public uint MapId { get; set; }

  [Column("node_id", TypeName = "int(10) unsigned")]
  public uint NodeId { get; set; }

  [Column("element_id", TypeName = "int(10) unsigned")]
  public uint ElementId { get; set; }

  [Column("type")]
  [StringLength(10)]
  public string Type { get; set; }
}
