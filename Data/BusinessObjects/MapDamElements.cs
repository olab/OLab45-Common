using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table( "map_dam_elements" )]
[Index( "DamId", Name = "dam_id" )]
[Index( "ElementId", Name = "element_id" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class MapDamElements
{
  [Key]
  [Column( "id", TypeName = "int(10) unsigned" )]
  public uint Id { get; set; }

  [Column( "dam_id", TypeName = "int(10) unsigned" )]
  public uint DamId { get; set; }

  [Column( "element_type" )]
  [StringLength( 20 )]
  public string ElementType { get; set; }

  [Column( "order", TypeName = "int(11)" )]
  public int? Order { get; set; }

  [Required]
  [Column( "display" )]
  [StringLength( 20 )]
  public string Display { get; set; }

  [Column( "element_id", TypeName = "int(10) unsigned" )]
  public uint ElementId { get; set; }

  [ForeignKey( "DamId" )]
  [InverseProperty( "MapDamElements" )]
  public virtual MapDams Dam { get; set; }
}
