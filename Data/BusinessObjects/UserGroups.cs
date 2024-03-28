using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("user_groups")]
[Index("GroupId", Name = "group_id")]
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

    [Required]
    [Column("role")]
    [StringLength(45)]
    public string Role { get; set; }

    [ForeignKey("GroupId")]
    [InverseProperty("UserGroups")]
    public virtual Groups Group { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserGroups")]
    public virtual Users User { get; set; }
}
