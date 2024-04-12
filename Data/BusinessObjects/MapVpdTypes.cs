using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_vpd_types")]
public partial class MapVpdTypes
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Required]
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [Column("label")]
    [StringLength(500)]
    public string Label { get; set; }

    [InverseProperty("VpdType")]
    public virtual ICollection<MapVpds> MapVpds { get; } = new List<MapVpds>();
}
