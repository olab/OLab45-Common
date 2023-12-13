using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Data.Models
{
  [Table("statistics_user_responses")]
  public partial class StatisticsUserResponses
  {
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }
    [Column("question_id", TypeName = "int(10) unsigned")]
    public uint QuestionId { get; set; }
    [Column("session_id", TypeName = "int(10) unsigned")]
    public uint SessionId { get; set; }
    [Required]
    [Column("response")]
    [StringLength(700)]
    public string Response { get; set; }
    [Column("node_id", TypeName = "int(10) unsigned")]
    public uint NodeId { get; set; }
  }
}
