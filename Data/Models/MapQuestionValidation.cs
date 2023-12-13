using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Data.Models
{
  [Table("map_question_validation")]
  [Index(nameof(QuestionId), Name = "question_id")]
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

    [ForeignKey(nameof(QuestionId))]
    [InverseProperty(nameof(MapQuestions.MapQuestionValidation))]
    public virtual MapQuestions Question { get; set; }
  }
}
