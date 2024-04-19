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
    [Column("id", TypeName = "int(10) unsigned")]
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

    [Column("language_id", TypeName = "int(10) unsigned")]
    public uint LanguageId { get; set; }

    [Column("type_id", TypeName = "int(10) unsigned")]
    public uint TypeId { get; set; }

    [Column("resetHashKey")]
    [StringLength(255)]
    public string ResetHashKey { get; set; }

    [Column("resetHashKeyTime", TypeName = "datetime")]
    public DateTime? ResetHashKeyTime { get; set; }

    [Column("resetAttempt", TypeName = "int(10)")]
    public int? ResetAttempt { get; set; }

    [Column("resetTimestamp", TypeName = "datetime")]
    public DateTime? ResetTimestamp { get; set; }

    [Column("visualEditorAutosaveTime", TypeName = "int(10)")]
    public int? VisualEditorAutosaveTime { get; set; }

    [Column("oauth_provider_id", TypeName = "int(10)")]
    public int? OauthProviderId { get; set; }

    [Column("oauth_id")]
    [StringLength(300)]
    public string OauthId { get; set; }

    [Column("history")]
    [StringLength(255)]
    public string History { get; set; }

    [Column("history_readonly")]
    public bool? HistoryReadonly { get; set; }

    [Column("history_timestamp", TypeName = "int(10)")]
    public int? HistoryTimestamp { get; set; }

    [Required]
    [Column("modeUI", TypeName = "enum('easy','advanced')")]
    public string ModeUi { get; set; }

    [Column("is_lti")]
    public bool? IsLti { get; set; }

    [Column("settings", TypeName = "text")]
    public string Settings { get; set; }

    [Required]
    [Column("group")]
    [StringLength(45)]
    public string Group { get; set; }

    [Required]
    [Column("role")]
    [StringLength(1024)]
    public string Role { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<MapUsers> MapUsers { get; } = new List<MapUsers>();

    [InverseProperty("User")]
    public virtual ICollection<UserBookmarks> UserBookmarks { get; } = new List<UserBookmarks>();

    [InverseProperty("User")]
    public virtual ICollection<UserGroups> UserGroups { get; } = new List<UserGroups>();

    [InverseProperty("User")]
    public virtual ICollection<UserNotes> UserNotes { get; } = new List<UserNotes>();

    [InverseProperty("User")]
    public virtual ICollection<WebinarUsers> WebinarUsers { get; } = new List<WebinarUsers>();
}
