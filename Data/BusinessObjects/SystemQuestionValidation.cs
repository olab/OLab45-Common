using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("system_question_validation")]
[Index("QuestionId", Name = "question_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class SystemQuestionValidation
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("question_id")]
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
    [InverseProperty("SystemQuestionValidation")]
    public virtual SystemQuestions Question { get; set; }
}
