using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table( "map_keys" )]
[Index( "Key", Name = "key" )]
[Index( "MapId", Name = "map_id" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class MapKeys
{
  [Key]
  [Column( "id", TypeName = "int(10) unsigned" )]
  public uint Id { get; set; }

  [Column( "map_id", TypeName = "int(10) unsigned" )]
  public uint MapId { get; set; }

  [Required]
  [Column( "key" )]
  [StringLength( 50 )]
  public string Key { get; set; }

  [ForeignKey( "MapId" )]
  [InverseProperty( "MapKeys" )]
  public virtual Maps Map { get; set; }
}
