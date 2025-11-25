using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_feedback_operators")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapFeedbackOperators
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Required]
    [Column("title")]
    [StringLength(100)]
    public string Title { get; set; }

    [Required]
    [Column("value")]
    [StringLength(50)]
    public string Value { get; set; }
}
