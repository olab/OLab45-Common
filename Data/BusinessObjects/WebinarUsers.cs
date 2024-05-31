using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table("webinar_users")]
[Index("UserId", Name = "user_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class WebinarUsers
{
  [Key]
  [Column("id", TypeName = "int(10) unsigned")]
  public uint Id { get; set; }

  [Column("webinar_id", TypeName = "int(10) unsigned")]
  public uint WebinarId { get; set; }

  [Column("user_id", TypeName = "int(10) unsigned")]
  public uint UserId { get; set; }

  [Column("include_4R")]
  public bool Include4r { get; set; }

  [Column("expert")]
  public bool Expert { get; set; }

  [ForeignKey("UserId")]
  [InverseProperty("WebinarUsers")]
  public virtual Users User { get; set; }
}
