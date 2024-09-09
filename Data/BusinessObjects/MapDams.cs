using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_dams")]
[Index("MapId", Name = "map_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapDams
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Column("map_id", TypeName = "int(10) unsigned")]
    public uint MapId { get; set; }

    [Column("name")]
    [StringLength(500)]
    public string Name { get; set; }

    [Column("is_private", TypeName = "int(4)")]
    public int IsPrivate { get; set; }

    [ForeignKey("MapId")]
    [InverseProperty("MapDams")]
    public virtual Maps Map { get; set; }

    [InverseProperty("Dam")]
    public virtual ICollection<MapDamElements> MapDamElements { get; } = new List<MapDamElements>();
}
