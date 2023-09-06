using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Api.Model
{
  [Table("map_feedback_operators")]
  public partial class MapFeedbackOperators
  {
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }
    [Required]
    [Column("title")]
    [StringLength(100)]
    public string Title { get; set; }
    [Required]
    [Column("value")]
    [StringLength(50)]
    public string Value { get; set; }
  }
}
