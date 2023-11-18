using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Api.Model
{
  [Table("h5p_tags")]
  public partial class H5pTags
  {
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }
    [Required]
    [Column("name")]
    [StringLength(31)]
    public string Name { get; set; }
  }
}
