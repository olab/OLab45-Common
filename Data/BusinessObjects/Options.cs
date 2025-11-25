using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("options")]
[Index("Name", Name = "name", IsUnique = true)]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class Options
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Required]
    [Column("name")]
    [StringLength(64)]
    public string Name { get; set; }

    [Required]
    [Column("value")]
    public string Value { get; set; }

    [Required]
    [Column("autoload")]
    [StringLength(20)]
    public string Autoload { get; set; }
}
