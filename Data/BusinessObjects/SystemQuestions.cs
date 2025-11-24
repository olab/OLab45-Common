using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("system_questions")]
[Index("ParentId", Name = "parent_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class SystemQuestions
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; }

    [Column("description", TypeName = "text")]
    public string Description { get; set; }

    [Column("parent_id")]
    public uint? ParentId { get; set; }

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

    [Column("show_answer", TypeName = "tinyint(1)")]
    public sbyte ShowAnswer { get; set; }

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

    [Column("imageable_id")]
    public uint ImageableId { get; set; }

    [Required]
    [Column("imageable_type")]
    [StringLength(45)]
    public string ImageableType { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_At", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Parent")]
    public virtual ICollection<SystemQuestions> InverseParent { get; set; } = new List<SystemQuestions>();

    [ForeignKey("ParentId")]
    [InverseProperty("InverseParent")]
    public virtual SystemQuestions Parent { get; set; }

    [InverseProperty("Question")]
    public virtual ICollection<SystemQuestionResponses> SystemQuestionResponses { get; set; } = new List<SystemQuestionResponses>();

    [InverseProperty("Question")]
    public virtual ICollection<SystemQuestionValidation> SystemQuestionValidation { get; set; } = new List<SystemQuestionValidation>();

    [InverseProperty("Question")]
    public virtual ICollection<UserResponses> UserResponses { get; set; } = new List<UserResponses>();
}
