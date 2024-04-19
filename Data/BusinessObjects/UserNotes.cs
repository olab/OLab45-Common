using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("user_notes")]
[Index("SessionId", Name = "session_id", IsUnique = true)]
[Index("UserId", Name = "user_id")]
[Index("WebinarId", Name = "webinar_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class UserNotes
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Column("user_id", TypeName = "int(10) unsigned")]
    public uint? UserId { get; set; }

    [Column("session_id", TypeName = "int(10) unsigned")]
    public uint? SessionId { get; set; }

    [Column("webinar_id", TypeName = "int(10) unsigned")]
    public uint? WebinarId { get; set; }

    [Required]
    [Column("text", TypeName = "text")]
    public string Text { get; set; }

    [Column("created_at")]
    [Precision(18, 6)]
    public decimal CreatedAt { get; set; }

    [Column("updated_at")]
    [Precision(18, 6)]
    public decimal UpdatedAt { get; set; }

    [Column("deleted_at")]
    [Precision(18, 6)]
    public decimal? DeletedAt { get; set; }

    [ForeignKey("SessionId")]
    [InverseProperty("UserNotes")]
    public virtual UserSessions Session { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserNotes")]
    public virtual Users User { get; set; }

    [ForeignKey("WebinarId")]
    [InverseProperty("UserNotes")]
    public virtual Webinars Webinar { get; set; }
}
