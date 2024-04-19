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
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Required]
    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; }

    [Column("author_id", TypeName = "int(10) unsigned")]
    public uint AuthorId { get; set; }

    [Required]
    [Column("abstract")]
    [StringLength(2000)]
    public string Abstract { get; set; }

    [Column("startScore", TypeName = "int(10)")]
    public int StartScore { get; set; }

    [Column("threshold", TypeName = "int(10)")]
    public int Threshold { get; set; }

    [Required]
    [Column("keywords")]
    [StringLength(500)]
    public string Keywords { get; set; }

    [Column("type_id", TypeName = "int(10) unsigned")]
    public uint TypeId { get; set; }

    [Required]
    [Column("units")]
    [StringLength(10)]
    public string Units { get; set; }

    [Column("security_id", TypeName = "int(10) unsigned")]
    public uint SecurityId { get; set; }

    [Required]
    [Column("guid")]
    [StringLength(50)]
    public string Guid { get; set; }

    [Column("timing")]
    public bool Timing { get; set; }

    [Column("delta_time", TypeName = "int(10)")]
    public int DeltaTime { get; set; }

    [Required]
    [Column("reminder_msg")]
    [StringLength(255)]
    public string ReminderMsg { get; set; }

    [Column("reminder_time", TypeName = "int(10)")]
    public int ReminderTime { get; set; }

    [Column("show_bar")]
    public bool ShowBar { get; set; }

    [Column("show_score")]
    public bool ShowScore { get; set; }

    [Column("skin_id", TypeName = "int(10) unsigned")]
    public uint SkinId { get; set; }

    [Column("enabled")]
    public bool Enabled { get; set; }

    [Column("section_id", TypeName = "int(10) unsigned")]
    public uint SectionId { get; set; }

    [Column("language_id", TypeName = "int(10) unsigned")]
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

    [Column("source_id", TypeName = "int(10) unsigned")]
    public uint SourceId { get; set; }

    [Column("verification", TypeName = "text")]
    public string Verification { get; set; }

    [Column("assign_forum_id", TypeName = "int(10)")]
    public int? AssignForumId { get; set; }

    [Column("author_rights", TypeName = "int(10)")]
    public int AuthorRights { get; set; }

    [Column("revisable_answers")]
    public bool RevisableAnswers { get; set; }

    [Column("send_xapi_statements")]
    public bool SendXapiStatements { get; set; }

    [Column("renderer_version")]
    public float? RendererVersion { get; set; }

    [Column("is_template", TypeName = "int(10)")]
    public int? IsTemplate { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_At", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column("report_node_id", TypeName = "int(10)")]
    public int? ReportNodeId { get; set; }

    [ForeignKey("LanguageId")]
    [InverseProperty("Maps")]
    public virtual Languages Language { get; set; }

    [InverseProperty("Map")]
    public virtual ICollection<MapAvatars> MapAvatars { get; } = new List<MapAvatars>();

    [InverseProperty("Map")]
    public virtual ICollection<MapChats> MapChats { get; } = new List<MapChats>();

    [InverseProperty("Map")]
    public virtual ICollection<MapCollectionMaps> MapCollectionMaps { get; } = new List<MapCollectionMaps>();

    [InverseProperty("Map")]
    public virtual ICollection<MapContributors> MapContributors { get; } = new List<MapContributors>();

    [InverseProperty("Map")]
    public virtual ICollection<MapCounterCommonRules> MapCounterCommonRules { get; } = new List<MapCounterCommonRules>();

    [InverseProperty("Map")]
    public virtual ICollection<MapCounters> MapCounters { get; } = new List<MapCounters>();

    [InverseProperty("Map")]
    public virtual ICollection<MapDams> MapDams { get; } = new List<MapDams>();

    [InverseProperty("Map")]
    public virtual ICollection<MapElements> MapElements { get; } = new List<MapElements>();

    [InverseProperty("Map")]
    public virtual ICollection<MapFeedbackRules> MapFeedbackRules { get; } = new List<MapFeedbackRules>();

    [InverseProperty("Map")]
    public virtual ICollection<MapKeys> MapKeys { get; } = new List<MapKeys>();

    [InverseProperty("Map")]
    public virtual ICollection<MapNodeJumps> MapNodeJumps { get; } = new List<MapNodeJumps>();

    [InverseProperty("Map")]
    public virtual ICollection<MapNodeLinks> MapNodeLinks { get; } = new List<MapNodeLinks>();

    [InverseProperty("Map")]
    public virtual ICollection<MapNodeSections> MapNodeSections { get; } = new List<MapNodeSections>();

    [InverseProperty("Map")]
    public virtual ICollection<MapNodes> MapNodes { get; } = new List<MapNodes>();

    [InverseProperty("Map")]
    public virtual ICollection<MapQuestions> MapQuestions { get; } = new List<MapQuestions>();

    [InverseProperty("Map")]
    public virtual ICollection<MapUsers> MapUsers { get; } = new List<MapUsers>();

    [InverseProperty("Map")]
    public virtual ICollection<QCumulative> QCumulative { get; } = new List<QCumulative>();

    [InverseProperty("Map")]
    public virtual ICollection<ScenarioMaps> ScenarioMaps { get; } = new List<ScenarioMaps>();

    [ForeignKey("SectionId")]
    [InverseProperty("Maps")]
    public virtual MapSections Section { get; set; }

    [ForeignKey("SecurityId")]
    [InverseProperty("Maps")]
    public virtual MapSecurities Security { get; set; }

    [InverseProperty("Map")]
    public virtual ICollection<SystemCounterActions> SystemCounterActions { get; } = new List<SystemCounterActions>();

    [ForeignKey("TypeId")]
    [InverseProperty("Maps")]
    public virtual MapTypes Type { get; set; }

    [InverseProperty("Map")]
    public virtual ICollection<UserSessions> UserSessions { get; } = new List<UserSessions>();

    [InverseProperty("Map")]
    public virtual ICollection<UserSessiontraces> UserSessiontraces { get; } = new List<UserSessiontraces>();

    [InverseProperty("Map")]
    public virtual ICollection<UserState> UserState { get; } = new List<UserState>();
}
