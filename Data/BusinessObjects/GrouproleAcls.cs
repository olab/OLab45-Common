using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table("grouprole_acls")]
[Index("GroupId", Name = "ifk_gra_group_idx")]
[Index("RoleId", Name = "ifk_gra_role_idx")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class GrouproleAcls
{
  [Key]
  [Column("id", TypeName = "int(10) unsigned")]
  public uint Id { get; set; }

  [Column("imageable_id", TypeName = "int(10) unsigned")]
  public uint? ImageableId { get; set; }

  [Column("imageable_type")]
  [StringLength(45)]
  public string ImageableType { get; set; }

  [Column("group_id", TypeName = "int(10) unsigned")]
  public uint? GroupId { get; set; }

  [Column("role_id", TypeName = "int(10) unsigned")]
  public uint? RoleId { get; set; }

  [Column("acl2", TypeName = "bit(3)")]
  public ulong Acl2 { get; set; }

  [ForeignKey("GroupId")]
  [InverseProperty("GrouproleAcls")]
  public virtual Groups Group { get; set; }

  [ForeignKey("RoleId")]
  [InverseProperty("GrouproleAcls")]
  public virtual Roles Role { get; set; }
}
