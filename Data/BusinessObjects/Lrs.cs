using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("lrs")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class Lrs
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("enabled", TypeName = "tinyint(1)")]
    public sbyte Enabled { get; set; }

    [Required]
    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; }

    [Required]
    [Column("url")]
    [StringLength(255)]
    public string Url { get; set; }

    [Required]
    [Column("username")]
    [StringLength(255)]
    public string Username { get; set; }

    [Required]
    [Column("password")]
    [StringLength(255)]
    public string Password { get; set; }

    [Column("api_version")]
    public byte ApiVersion { get; set; }

    [InverseProperty("Lrs")]
    public virtual ICollection<LrsStatement> LrsStatement { get; set; } = new List<LrsStatement>();
}
