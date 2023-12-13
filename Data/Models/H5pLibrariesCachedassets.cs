using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Data.Models
{
  [Table("h5p_libraries_cachedassets")]
  public partial class H5pLibrariesCachedassets
  {
    [Key]
    [Column("library_id", TypeName = "int(10) unsigned")]
    public uint LibraryId { get; set; }
    [Key]
    [Column("hash")]
    [StringLength(64)]
    public string Hash { get; set; }
  }
}
