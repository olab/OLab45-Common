using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("grouprole_acls")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class GrouproleAcls
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Required]
    [Column("name")]
    [StringLength(45)]
    public string Name { get; set; }

    [Column("imageable_id", TypeName = "int(10) unsigned")]
    public uint ImageableId { get; set; }

    [Required]
    [Column("imageable_type")]
    [StringLength(45)]
    public string ImageableType { get; set; }

    [Required]
    [Column("acl")]
    [StringLength(45)]
    public string Acl { get; set; }

    [Column("group_id", TypeName = "int(10) unsigned")]
    public uint GroupId { get; set; }

    [Column("role_id", TypeName = "int(10) unsigned")]
    public uint RoleId { get; set; }

    [Column("acl2", TypeName = "bit(3)")]
    public ulong Acl2 { get; set; }
}
