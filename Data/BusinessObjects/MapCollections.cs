using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table( "map_collections" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class MapCollections
{
  [Key]
  [Column( "id", TypeName = "int(10) unsigned" )]
  public uint Id { get; set; }

  [Column( "name" )]
  [StringLength( 200 )]
  public string Name { get; set; }

  [InverseProperty( "Collection" )]
  public virtual ICollection<MapCollectionmaps> MapCollectionmaps { get; set; } = new List<MapCollectionmaps>();
}
