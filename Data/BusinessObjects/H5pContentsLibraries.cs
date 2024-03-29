using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Api.Model;

[Table("h5p_contents_libraries")]
public partial class H5pContentsLibraries
{
  [Key]
  [Column("content_id", TypeName = "int(10) unsigned")]
  public uint ContentId { get; set; }
  [Key]
  [Column("library_id", TypeName = "int(10) unsigned")]
  public uint LibraryId { get; set; }
  [Key]
  [Column("dependency_type")]
  [StringLength(31)]
  public string DependencyType { get; set; }
  [Column("weight", TypeName = "smallint(5) unsigned")]
  public ushort Weight { get; set; }
  [Column("drop_css", TypeName = "tinyint(3) unsigned")]
  public byte DropCss { get; set; }
}
