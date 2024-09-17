using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table("roles")]
[Index("Name", Name = "name")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class Roles
{
  [Key]
  [Column("id", TypeName = "int(10) unsigned")]
  public uint Id { get; set; }

  [Column("description")]
  [StringLength(100)]
  public string Description { get; set; }

  [Required]
  [Column("name")]
  [StringLength(100)]
  public string Name { get; set; }

  [Column("is_system", TypeName = "tinyint(4)")]
  public sbyte? IsSystem { get; set; }

  [InverseProperty("Role")]
  public virtual ICollection<GrouproleAcls> GrouproleAcls { get; } = new List<GrouproleAcls>();

  [InverseProperty("Role")]
  public virtual ICollection<MapGrouproles> MapGrouproles { get; } = new List<MapGrouproles>();

  [InverseProperty("Role")]
  public virtual ICollection<MapNodeGrouproles> MapNodeGrouproles { get; } = new List<MapNodeGrouproles>();

  [InverseProperty("Role")]
  public virtual ICollection<UserGrouproles> UserGrouproles { get; } = new List<UserGrouproles>();
}
