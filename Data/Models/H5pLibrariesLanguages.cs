using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Data.Models
{
  [Table("h5p_libraries_languages")]
  public partial class H5pLibrariesLanguages
  {
    [Key]
    [Column("library_id", TypeName = "int(10) unsigned")]
    public uint LibraryId { get; set; }
    [Key]
    [Column("language_code")]
    [StringLength(31)]
    public string LanguageCode { get; set; }
    [Required]
    [Column("translation", TypeName = "text")]
    public string Translation { get; set; }
  }
}
