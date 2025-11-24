using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("system_courses")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class SystemCourses
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Required]
    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; }

    [Column("description")]
    [StringLength(200)]
    public string Description { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_At", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }
}
