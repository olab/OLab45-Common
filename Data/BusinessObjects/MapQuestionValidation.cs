using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_question_validation")]
[Index("QuestionId", Name = "question_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapQuestionValidation
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Column("question_id", TypeName = "int(10) unsigned")]
    public uint QuestionId { get; set; }

    [Required]
    [Column("validator", TypeName = "text")]
    public string Validator { get; set; }

    [Required]
    [Column("second_parameter", TypeName = "text")]
    public string SecondParameter { get; set; }

    [Required]
    [Column("error_message", TypeName = "text")]
    public string ErrorMessage { get; set; }

    [ForeignKey("QuestionId")]
    [InverseProperty("MapQuestionValidation")]
    public virtual MapQuestions Question { get; set; }
}
