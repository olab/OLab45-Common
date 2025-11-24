using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("webinar_users")]
[Index("UserId", Name = "user_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class WebinarUsers
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("webinar_id")]
    public uint WebinarId { get; set; }

    [Column("user_id")]
    public uint UserId { get; set; }

    [Column("include_4R", TypeName = "tinyint(1)")]
    public sbyte Include4r { get; set; }

    [Column("expert", TypeName = "tinyint(1)")]
    public sbyte Expert { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("WebinarUsers")]
    public virtual Users User { get; set; }
}
