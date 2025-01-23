using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table( "webinar_maps" )]
[Index( "Step", Name = "step" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class WebinarMaps
{
  [Key]
  [Column( "id", TypeName = "int(10) unsigned" )]
  public uint Id { get; set; }

  [Column( "webinar_id", TypeName = "int(10) unsigned" )]
  public uint WebinarId { get; set; }

  [Required]
  [Column( "which", TypeName = "enum('labyrinth','section')" )]
  public string Which { get; set; }

  [Column( "reference_id", TypeName = "int(10) unsigned" )]
  public uint ReferenceId { get; set; }

  [Column( "step", TypeName = "int(10) unsigned" )]
  public uint Step { get; set; }

  [Column( "cumulative" )]
  public bool Cumulative { get; set; }

  [ForeignKey( "Step" )]
  [InverseProperty( "WebinarMaps" )]
  public virtual WebinarSteps StepNavigation { get; set; }
}
