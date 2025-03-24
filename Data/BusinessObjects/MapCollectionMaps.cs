using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table( "map_collectionmaps" )]
[Index( "CollectionId", Name = "collection_id" )]
[Index( "MapId", Name = "map_id" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class MapCollectionmaps
{
  [Key]
  [Column( "id", TypeName = "int(10) unsigned" )]
  public uint Id { get; set; }

  [Column( "collection_id", TypeName = "int(10) unsigned" )]
  public uint CollectionId { get; set; }

  [Column( "map_id", TypeName = "int(10) unsigned" )]
  public uint MapId { get; set; }

  [ForeignKey( "CollectionId" )]
  [InverseProperty( "MapCollectionmaps" )]
  public virtual MapCollections Collection { get; set; }

  [ForeignKey( "MapId" )]
  [InverseProperty( "MapCollectionmaps" )]
  public virtual Maps Map { get; set; }
}
