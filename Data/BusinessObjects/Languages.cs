using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table( "languages" )]
[Index( "Name", Name = "name" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class Languages
{
  [Key]
  [Column( "id", TypeName = "int(10) unsigned" )]
  public uint Id { get; set; }

  [Required]
  [Column( "name" )]
  [StringLength( 20 )]
  public string Name { get; set; }

  [Required]
  [Column( "key" )]
  [StringLength( 20 )]
  public string Key { get; set; }

  [InverseProperty( "Language" )]
  public virtual ICollection<Maps> Maps { get; } = new List<Maps>();
}
