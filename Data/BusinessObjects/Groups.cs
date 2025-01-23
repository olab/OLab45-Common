using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table( "groups" )]
[Index( "Name", Name = "name" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class Groups
{
  [Key]
  [Column( "id", TypeName = "int(10) unsigned" )]
  public uint Id { get; set; }

  [Required]
  [Column( "name" )]
  [StringLength( 100 )]
  public string Name { get; set; }

  [Column( "is_system", TypeName = "tinyint(4)" )]
  public sbyte IsSystem { get; set; }

  [InverseProperty( "Group" )]
  public virtual ICollection<GrouproleAcls> GrouproleAcls { get; } = new List<GrouproleAcls>();

  [InverseProperty( "Group" )]
  public virtual ICollection<MapGrouproles> MapGrouproles { get; } = new List<MapGrouproles>();

  [InverseProperty( "Group" )]
  public virtual ICollection<MapNodeGrouproles> MapNodeGrouproles { get; } = new List<MapNodeGrouproles>();

  [InverseProperty( "Group" )]
  public virtual ICollection<UserGrouproles> UserGrouproles { get; } = new List<UserGrouproles>();
}
