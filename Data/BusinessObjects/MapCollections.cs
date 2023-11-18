using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Api.Model
{
  [Table("map_collections")]
  public partial class MapCollections
  {
    public MapCollections()
    {
      MapCollectionMaps = new HashSet<MapCollectionMaps>();
    }

    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }
    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; }

    [InverseProperty("Collection")]
    public virtual ICollection<MapCollectionMaps> MapCollectionMaps { get; set; }
  }
}
