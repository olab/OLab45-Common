using Microsoft.EntityFrameworkCore;
using OLab.Api.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Data.BusinessObjects.API
{
    [Table("system_question_validation")]
    [Index(nameof(QuestionId), Name = "question_id")]
    public partial class SystemQuestionValidation
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

        [ForeignKey(nameof(QuestionId))]
        [InverseProperty(nameof(SystemQuestions.SystemQuestionValidation))]
        public virtual SystemQuestions Question { get; set; }
    }
}
