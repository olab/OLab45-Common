using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table("map_groups")]
[Index("GroupId", Name = "group_id")]
[Index("MapId", Name = "map_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapGroups
{
  [Key]
  [Column("id", TypeName = "int(10) unsigned")]
  public uint Id { get; set; }

  [Column("map_id", TypeName = "int(10) unsigned")]
  public uint MapId { get; set; }

  [Column("group_id", TypeName = "int(10) unsigned")]
  public uint GroupId { get; set; }

  [ForeignKey("GroupId")]
  [InverseProperty("MapGroups")]
  public virtual Groups Group { get; set; }

  [ForeignKey("MapId")]
  [InverseProperty("MapGroups")]
  public virtual Maps Map { get; set; }
}
