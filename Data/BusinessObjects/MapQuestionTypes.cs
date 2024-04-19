using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_question_types")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapQuestionTypes
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Column("title")]
    [StringLength(70)]
    public string Title { get; set; }

    [Column("value")]
    [StringLength(20)]
    public string Value { get; set; }

    [Required]
    [Column("template_name")]
    [StringLength(200)]
    public string TemplateName { get; set; }

    [Column("template_args")]
    [StringLength(100)]
    public string TemplateArgs { get; set; }
}
