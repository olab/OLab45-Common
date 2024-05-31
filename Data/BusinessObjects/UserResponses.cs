using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table("user_responses")]
[Index("QuestionId", Name = "question_id")]
[Index("SessionId", Name = "session_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class UserResponses
{
  [Key]
  [Column("id", TypeName = "int(10) unsigned")]
  public uint Id { get; set; }

  [Column("question_id", TypeName = "int(10) unsigned")]
  public uint QuestionId { get; set; }

  [Column("session_id", TypeName = "int(10) unsigned")]
  public uint SessionId { get; set; }

  [Column("response")]
  [StringLength(1000)]
  public string Response { get; set; }

  [Column("node_id", TypeName = "int(10) unsigned")]
  public uint NodeId { get; set; }

  [Column("created_at")]
  [Precision(18, 6)]
  public decimal CreatedAt { get; set; }

  [ForeignKey("QuestionId")]
  [InverseProperty("UserResponses")]
  public virtual SystemQuestions Question { get; set; }

  [ForeignKey("SessionId")]
  [InverseProperty("UserResponses")]
  public virtual UserSessions Session { get; set; }

  [InverseProperty("Userresponse")]
  public virtual ICollection<UserresponseCounterupdate> UserresponseCounterupdate { get; } = new List<UserresponseCounterupdate>();
}
