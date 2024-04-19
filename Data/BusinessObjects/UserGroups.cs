using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("user_groups")]
[Index("GroupId", Name = "group_id")]
[Index("RoleId", Name = "user_groups_ibfk_3")]
[Index("UserId", Name = "user_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class UserGroups
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Required]
    [Column("iss")]
    [StringLength(45)]
    public string Iss { get; set; }

    [Column("user_id", TypeName = "int(10) unsigned")]
    public uint UserId { get; set; }

    [Column("group_id", TypeName = "int(10) unsigned")]
    public uint GroupId { get; set; }

    [Column("role_id", TypeName = "int(10) unsigned")]
    public uint RoleId { get; set; }

    [ForeignKey("GroupId")]
    [InverseProperty("UserGroups")]
    public virtual Groups Group { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("UserGroups")]
    public virtual Roles Role { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserGroups")]
    public virtual Users User { get; set; }
}
