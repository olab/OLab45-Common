using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("maps")]
[Index("AuthorId", Name = "author_id")]
[Index("AuthorId", "TypeId", "SecurityId", "SectionId", "LanguageId", Name = "author_id_2")]
[Index("LanguageId", Name = "language_id")]
[Index("SectionId", Name = "section_id")]
[Index("SecurityId", Name = "security_id")]
[Index("SkinId", Name = "skin_id")]
[Index("TypeId", "SkinId", "SectionId", "LanguageId", Name = "type_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class Maps
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Required]
    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; }

    [Column("author_id")]
    public uint AuthorId { get; set; }

    [Required]
    [Column("abstract")]
    [StringLength(2000)]
    public string Abstract { get; set; }

    [Column("startScore")]
    public int StartScore { get; set; }

    [Column("threshold")]
    public int Threshold { get; set; }

    [Required]
    [Column("keywords")]
    [StringLength(500)]
    public string Keywords { get; set; }

    [Column("type_id")]
    public uint TypeId { get; set; }

    [Required]
    [Column("units")]
    [StringLength(10)]
    public string Units { get; set; }

    [Column("security_id")]
    public uint SecurityId { get; set; }

    [Required]
    [Column("guid")]
    [StringLength(50)]
    public string Guid { get; set; }

    [Column("timing", TypeName = "tinyint(1)")]
    public sbyte Timing { get; set; }

    [Column("delta_time")]
    public int DeltaTime { get; set; }

    [Required]
    [Column("reminder_msg")]
    [StringLength(255)]
    public string ReminderMsg { get; set; }

    [Column("reminder_time")]
    public int ReminderTime { get; set; }

    [Column("show_bar", TypeName = "tinyint(1)")]
    public sbyte ShowBar { get; set; }

    [Column("show_score", TypeName = "tinyint(1)")]
    public sbyte ShowScore { get; set; }

    [Column("skin_id")]
    public uint SkinId { get; set; }

    [Column("enabled", TypeName = "tinyint(1)")]
    public sbyte Enabled { get; set; }

    [Column("section_id")]
    public uint SectionId { get; set; }

    [Column("language_id")]
    public uint? LanguageId { get; set; }

    [Required]
    [Column("feedback")]
    [StringLength(2000)]
    public string Feedback { get; set; }

    [Required]
    [Column("dev_notes")]
    [StringLength(1000)]
    public string DevNotes { get; set; }

    [Required]
    [Column("source")]
    [StringLength(50)]
    public string Source { get; set; }

    [Column("source_id")]
    public uint SourceId { get; set; }

    [Column("verification", TypeName = "text")]
    public string Verification { get; set; }

    [Column("assign_forum_id")]
    public int? AssignForumId { get; set; }

    [Column("author_rights")]
    public int AuthorRights { get; set; }

    [Column("revisable_answers", TypeName = "tinyint(1)")]
    public sbyte RevisableAnswers { get; set; }

    [Column("send_xapi_statements", TypeName = "tinyint(1)")]
    public sbyte SendXapiStatements { get; set; }

    [Column("renderer_version")]
    public float? RendererVersion { get; set; }

    [Column("is_template")]
    public int? IsTemplate { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_At", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column("report_node_id")]
    public int? ReportNodeId { get; set; }

    [ForeignKey("LanguageId")]
    [InverseProperty("Maps")]
    public virtual Languages Language { get; set; }

    [InverseProperty("Map")]
    public virtual ICollection<MapAvatars> MapAvatars { get; set; } = new List<MapAvatars>();

    [InverseProperty("Map")]
    public virtual ICollection<MapChats> MapChats { get; set; } = new List<MapChats>();

    [InverseProperty("Map")]
    public virtual ICollection<MapCollectionmaps> MapCollectionmaps { get; set; } = new List<MapCollectionmaps>();

    [InverseProperty("Map")]
    public virtual ICollection<MapContributors> MapContributors { get; set; } = new List<MapContributors>();

    [InverseProperty("Map")]
    public virtual ICollection<MapCounterCommonRules> MapCounterCommonRules { get; set; } = new List<MapCounterCommonRules>();

    [InverseProperty("Map")]
    public virtual ICollection<MapCounters> MapCounters { get; set; } = new List<MapCounters>();

    [InverseProperty("Map")]
    public virtual ICollection<MapDams> MapDams { get; set; } = new List<MapDams>();

    [InverseProperty("Map")]
    public virtual ICollection<MapElements> MapElements { get; set; } = new List<MapElements>();

    [InverseProperty("Map")]
    public virtual ICollection<MapFeedbackRules> MapFeedbackRules { get; set; } = new List<MapFeedbackRules>();

    [InverseProperty("Map")]
    public virtual ICollection<MapGrouproles> MapGrouproles { get; set; } = new List<MapGrouproles>();

    [InverseProperty("Map")]
    public virtual ICollection<MapGroups> MapGroups { get; set; } = new List<MapGroups>();

    [InverseProperty("Map")]
    public virtual ICollection<MapKeys> MapKeys { get; set; } = new List<MapKeys>();

    [InverseProperty("Map")]
    public virtual ICollection<MapNodeJumps> MapNodeJumps { get; set; } = new List<MapNodeJumps>();

    [InverseProperty("Map")]
    public virtual ICollection<MapNodeLinks> MapNodeLinks { get; set; } = new List<MapNodeLinks>();

    [InverseProperty("Map")]
    public virtual ICollection<MapNodeSections> MapNodeSections { get; set; } = new List<MapNodeSections>();

    [InverseProperty("Map")]
    public virtual ICollection<MapNodes> MapNodes { get; set; } = new List<MapNodes>();

    [InverseProperty("Map")]
    public virtual ICollection<MapNodesCopy> MapNodesCopy { get; set; } = new List<MapNodesCopy>();

    [InverseProperty("Map")]
    public virtual ICollection<MapQuestions> MapQuestions { get; set; } = new List<MapQuestions>();

    [InverseProperty("Map")]
    public virtual ICollection<MapUsers> MapUsers { get; set; } = new List<MapUsers>();

    [InverseProperty("Map")]
    public virtual ICollection<QCumulative> QCumulative { get; set; } = new List<QCumulative>();

    [InverseProperty("Map")]
    public virtual ICollection<ScenarioMaps> ScenarioMaps { get; set; } = new List<ScenarioMaps>();

    [ForeignKey("SectionId")]
    [InverseProperty("Maps")]
    public virtual MapSections Section { get; set; }

    [ForeignKey("SecurityId")]
    [InverseProperty("Maps")]
    public virtual MapSecurities Security { get; set; }

    [InverseProperty("Map")]
    public virtual ICollection<SystemCounterActions> SystemCounterActions { get; set; } = new List<SystemCounterActions>();

    [ForeignKey("TypeId")]
    [InverseProperty("Maps")]
    public virtual MapTypes Type { get; set; }

    [InverseProperty("Map")]
    public virtual ICollection<UserSessions> UserSessions { get; set; } = new List<UserSessions>();

    [InverseProperty("Map")]
    public virtual ICollection<UserSessiontraces> UserSessiontraces { get; set; } = new List<UserSessiontraces>();

    [InverseProperty("Map")]
    public virtual ICollection<UserState> UserState { get; set; } = new List<UserState>();
}
