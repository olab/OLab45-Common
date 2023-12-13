using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Data.Models
{
  [Table("h5p_contents_tags")]
  public partial class H5pContentsTags
  {
    [Key]
    [Column("content_id", TypeName = "int(10) unsigned")]
    public uint ContentId { get; set; }
    [Key]
    [Column("tag_id", TypeName = "int(10) unsigned")]
    public uint TagId { get; set; }
  }
}
