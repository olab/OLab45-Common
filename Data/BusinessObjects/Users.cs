using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("users")]
[Index("LanguageId", Name = "fk_language_id")]
[Index("TypeId", Name = "fk_type_id")]
[Index("Username", "Email", Name = "username", IsUnique = true)]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class Users
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Required]
    [Column("username")]
    public string Username { get; set; }

    [Required]
    [Column("password")]
    [StringLength(800)]
    public string Password { get; set; }

    [Required]
    [Column("salt")]
    [StringLength(64)]
    public string Salt { get; set; }

    [Required]
    [Column("email")]
    [StringLength(250)]
    public string Email { get; set; }

    [Required]
    [Column("nickname")]
    [StringLength(120)]
    public string Nickname { get; set; }

    [Column("language_id")]
    public uint LanguageId { get; set; }

    [Column("type_id")]
    public uint TypeId { get; set; }

    [Column("resetHashKey")]
    [StringLength(255)]
    public string ResetHashKey { get; set; }

    [Column("resetHashKeyTime", TypeName = "datetime")]
    public DateTime? ResetHashKeyTime { get; set; }

    [Column("resetAttempt")]
    public int? ResetAttempt { get; set; }

    [Column("resetTimestamp", TypeName = "datetime")]
    public DateTime? ResetTimestamp { get; set; }

    [Column("visualEditorAutosaveTime")]
    public int? VisualEditorAutosaveTime { get; set; }

    [Column("oauth_provider_id")]
    public int? OauthProviderId { get; set; }

    [Column("oauth_id")]
    [StringLength(300)]
    public string OauthId { get; set; }

    [Column("history")]
    [StringLength(255)]
    public string History { get; set; }

    [Column("history_readonly", TypeName = "tinyint(1)")]
    public sbyte? HistoryReadonly { get; set; }

    [Column("history_timestamp")]
    public int? HistoryTimestamp { get; set; }

    [Required]
    [Column("modeUI", TypeName = "enum('easy','advanced')")]
    public string ModeUi { get; set; }

    [Column("lti", TypeName = "tinyint(1)")]
    public sbyte? Lti { get; set; }

    [Column("settings", TypeName = "text")]
    public string Settings { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<MapUsers> MapUsers { get; set; } = new List<MapUsers>();

    [InverseProperty("User")]
    public virtual ICollection<UserBookmarks> UserBookmarks { get; set; } = new List<UserBookmarks>();

    [InverseProperty("User")]
    public virtual ICollection<UserGrouproles> UserGrouproles { get; set; } = new List<UserGrouproles>();

    [InverseProperty("User")]
    public virtual ICollection<UserNotes> UserNotes { get; set; } = new List<UserNotes>();

    [InverseProperty("User")]
    public virtual ICollection<WebinarUsers> WebinarUsers { get; set; } = new List<WebinarUsers>();
}
