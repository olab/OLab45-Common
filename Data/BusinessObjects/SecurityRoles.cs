using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("security_roles")]
[Index("GroupId", Name = "security_roles_ibfk_1")]
[Index("RoleId", Name = "security_roles_ibfk_2")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class SecurityRoles
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

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

    [ForeignKey("GroupId")]
    [InverseProperty("SecurityRoles")]
    public virtual Groups Group { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("SecurityRoles")]
    public virtual Roles Role { get; set; }
}
