using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("user_grouproles")]
[Index("UserId", Name = "user_grouproles_ibfk_1")]
[Index("GroupId", Name = "user_grouproles_ibfk_2")]
[Index("RoleId", Name = "user_grouproles_ibfk_3")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class UserGrouproles
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

    [Column("role_id", TypeName = "int(10) unsigned")]
    public uint RoleId { get; set; }

    [Column("group_id", TypeName = "int(10) unsigned")]
    public uint GroupId { get; set; }

    [ForeignKey("GroupId")]
    [InverseProperty("UserGrouproles")]
    public virtual Groups Group { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("UserGrouproles")]
    public virtual Roles Role { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserGrouproles")]
    public virtual Users User { get; set; }
}
