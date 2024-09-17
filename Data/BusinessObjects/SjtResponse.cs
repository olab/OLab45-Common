using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table("sjt_response")]
[Index("ResponseId", Name = "response_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class SjtResponse
{
  [Key]
  [Column("id", TypeName = "int(10) unsigned")]
  public uint Id { get; set; }

  [Column("response_id", TypeName = "int(10) unsigned")]
  public uint ResponseId { get; set; }

  [Column("position", TypeName = "int(10)")]
  public int Position { get; set; }

  [Column("points", TypeName = "int(10)")]
  public int Points { get; set; }

  [ForeignKey("ResponseId")]
  [InverseProperty("SjtResponse")]
  public virtual MapQuestionResponses Response { get; set; }
}
