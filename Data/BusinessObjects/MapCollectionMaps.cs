using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_collectionMaps")]
[Index("CollectionId", Name = "collection_id")]
[Index("MapId", Name = "map_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapCollectionMaps
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Column("collection_id", TypeName = "int(10) unsigned")]
    public uint CollectionId { get; set; }

    [Column("map_id", TypeName = "int(10) unsigned")]
    public uint MapId { get; set; }

    [ForeignKey("CollectionId")]
    [InverseProperty("MapCollectionMaps")]
    public virtual MapCollections Collection { get; set; }

    [ForeignKey("MapId")]
    [InverseProperty("MapCollectionMaps")]
    public virtual Maps Map { get; set; }
}
