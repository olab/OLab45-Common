using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Api.Model;

[Table("options")]
[Index(nameof(Name), Name = "name", IsUnique = true)]
public partial class Options
{
  [Key]
  [Column("id", TypeName = "int(10) unsigned")]
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
