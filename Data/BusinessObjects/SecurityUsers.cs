using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("security_users")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class SecurityUsers
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Column("user_id", TypeName = "int(10)")]
    public int? UserId { get; set; }

    [Column("iss")]
    [StringLength(45)]
    public string Iss { get; set; }

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
}
