using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_sections")]
public partial class MapSections
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Required]
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [Column("description")]
    [StringLength(700)]
    public string Description { get; set; }

    [InverseProperty("Section")]
    public virtual ICollection<Maps> Maps { get; } = new List<Maps>();
}
