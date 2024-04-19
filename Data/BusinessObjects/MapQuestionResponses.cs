using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_question_responses")]
[Index("ParentId", Name = "parent_id")]
[Index("QuestionId", Name = "question_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapQuestionResponses
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Column("parent_id", TypeName = "int(10) unsigned")]
    public uint? ParentId { get; set; }

    [Column("question_id", TypeName = "int(10) unsigned")]
    public uint? QuestionId { get; set; }

    [Column("response")]
    [StringLength(250)]
    public string Response { get; set; }

    [Column("feedback", TypeName = "text")]
    public string Feedback { get; set; }

    [Column("is_correct")]
    public bool IsCorrect { get; set; }

    [Column("score", TypeName = "int(10)")]
    public int? Score { get; set; }

    [Column("from")]
    [StringLength(200)]
    public string From { get; set; }

    [Column("to")]
    [StringLength(200)]
    public string To { get; set; }

    [Column("order", TypeName = "int(10) unsigned")]
    public uint Order { get; set; }

    [InverseProperty("Parent")]
    public virtual ICollection<MapQuestionResponses> InverseParent { get; } = new List<MapQuestionResponses>();

    [ForeignKey("ParentId")]
    [InverseProperty("InverseParent")]
    public virtual MapQuestionResponses Parent { get; set; }

    [ForeignKey("QuestionId")]
    [InverseProperty("MapQuestionResponses")]
    public virtual MapQuestions Question { get; set; }

    [InverseProperty("Response")]
    public virtual ICollection<SjtResponse> SjtResponse { get; } = new List<SjtResponse>();
}
