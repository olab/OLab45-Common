using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("webinar_maps")]
[Index("Step", Name = "step")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class WebinarMaps
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("webinar_id")]
    public uint WebinarId { get; set; }

    [Required]
    [Column("which", TypeName = "enum('labyrinth','section')")]
    public string Which { get; set; }

    [Column("reference_id")]
    public uint ReferenceId { get; set; }

    [Column("step")]
    public uint Step { get; set; }

    [Column("cumulative", TypeName = "tinyint(1)")]
    public sbyte Cumulative { get; set; }

    [ForeignKey("Step")]
    [InverseProperty("WebinarMaps")]
    public virtual WebinarSteps StepNavigation { get; set; }
}
