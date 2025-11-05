using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_questions")]
[Index("MapId", Name = "map_id")]
[Index("ParentId", Name = "parent_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapQuestions
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("parent_id")]
    public uint? ParentId { get; set; }

    [Column("map_id")]
    public uint? MapId { get; set; }

    [Column("stem")]
    [StringLength(500)]
    public string Stem { get; set; }

    [Column("entry_type_id")]
    public uint EntryTypeId { get; set; }

    [Column("width")]
    public int Width { get; set; }

    [Column("height")]
    public int Height { get; set; }

    [Column("feedback")]
    [StringLength(1000)]
    public string Feedback { get; set; }

    [Required]
    [Column("prompt", TypeName = "text")]
    public string Prompt { get; set; }

    [Column("show_answer")]
    public bool ShowAnswer { get; set; }

    [Column("counter_id")]
    public uint? CounterId { get; set; }

    [Column("num_tries")]
    public int NumTries { get; set; }

    [Column("show_submit")]
    public sbyte ShowSubmit { get; set; }

    [Column("redirect_node_id")]
    public uint? RedirectNodeId { get; set; }

    [Column("submit_text")]
    [StringLength(200)]
    public string SubmitText { get; set; }

    [Column("type_display")]
    public int TypeDisplay { get; set; }

    [Column("settings", TypeName = "text")]
    public string Settings { get; set; }

    [Column("is_private")]
    public int IsPrivate { get; set; }

    [Column("order")]
    public int? Order { get; set; }

    [Column("external_source_id")]
    [StringLength(255)]
    public string ExternalSourceId { get; set; }

    [InverseProperty("Parent")]
    public virtual ICollection<MapQuestions> InverseParent { get; set; } = new List<MapQuestions>();

    [ForeignKey("MapId")]
    [InverseProperty("MapQuestions")]
    public virtual Maps Map { get; set; }

    [InverseProperty("Question")]
    public virtual ICollection<MapQuestionResponses> MapQuestionResponses { get; set; } = new List<MapQuestionResponses>();

    [InverseProperty("Question")]
    public virtual ICollection<MapQuestionValidation> MapQuestionValidation { get; set; } = new List<MapQuestionValidation>();

    [ForeignKey("ParentId")]
    [InverseProperty("InverseParent")]
    public virtual MapQuestions Parent { get; set; }

    [InverseProperty("Question")]
    public virtual ICollection<QCumulative> QCumulative { get; set; } = new List<QCumulative>();
}
