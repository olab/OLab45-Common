using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Model
{
  [Table("map_feedback_types")]
  public partial class MapFeedbackTypes
  {
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; }
    [Column("description", TypeName = "text")]
    public string Description { get; set; }
  }
}
