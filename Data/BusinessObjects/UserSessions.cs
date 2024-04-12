using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("user_sessions")]
[Index("Id", Name = "id", IsUnique = true)]
[Index("MapId", Name = "map_id")]
[Index("UserId", Name = "user_id")]
[Index("Uuid", Name = "uuid", IsUnique = true)]
public partial class UserSessions
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Required]
    [Column("uuid")]
    public string Uuid { get; set; }

    [Required]
    [Column("iss")]
    [StringLength(45)]
    public string Iss { get; set; }

    [Column("user_id", TypeName = "int(10) unsigned")]
    public uint UserId { get; set; }

    [Column("map_id", TypeName = "int(10) unsigned")]
    public uint MapId { get; set; }

    [Column("course_name")]
    [StringLength(45)]
    public string CourseName { get; set; }

    [Column("start_time")]
    [Precision(18, 6)]
    public decimal StartTime { get; set; }

    [Required]
    [Column("user_ip")]
    [StringLength(50)]
    public string UserIp { get; set; }

    [Column("webinar_id", TypeName = "int(10)")]
    public int? WebinarId { get; set; }

    [Column("webinar_step", TypeName = "int(10)")]
    public int? WebinarStep { get; set; }

    [Column("notCumulative")]
    public bool NotCumulative { get; set; }

    [Column("reset_at")]
    [Precision(18, 6)]
    public decimal? ResetAt { get; set; }

    [Column("end_time")]
    [Precision(18, 6)]
    public decimal? EndTime { get; set; }

    [ForeignKey("MapId")]
    [InverseProperty("UserSessions")]
    public virtual Maps Map { get; set; }

    [InverseProperty("Session")]
    public virtual ICollection<Statements> Statements { get; } = new List<Statements>();

    [InverseProperty("Session")]
    public virtual ICollection<UserBookmarks> UserBookmarks { get; } = new List<UserBookmarks>();

    [InverseProperty("Session")]
    public virtual UserNotes UserNotes { get; set; }

    [InverseProperty("Session")]
    public virtual ICollection<UserResponses> UserResponses { get; } = new List<UserResponses>();

    [InverseProperty("Session")]
    public virtual ICollection<UserSessiontraces> UserSessiontraces { get; } = new List<UserSessiontraces>();
}
