using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Data.Models;

[Table("map_counter_rule_relations")]
public partial class MapCounterRuleRelations
{
  [Key]
  [Column("id", TypeName = "int(10) unsigned")]
  public uint Id { get; set; }
  [Required]
  [Column("title")]
  [StringLength(70)]
  public string Title { get; set; }
  [Required]
  [Column("value")]
  [StringLength(50)]
  public string Value { get; set; }
}
