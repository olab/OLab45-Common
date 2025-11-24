using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_collections")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapCollections
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; }

    [InverseProperty("Collection")]
    public virtual ICollection<MapCollectionmaps> MapCollectionmaps { get; set; } = new List<MapCollectionmaps>();
}
