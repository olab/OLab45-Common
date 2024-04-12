using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_vpd_elements")]
[Index("VpdId", Name = "vpd_id")]
public partial class MapVpdElements
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Column("vpd_id", TypeName = "int(10) unsigned")]
    public uint VpdId { get; set; }

    [Required]
    [Column("key")]
    [StringLength(100)]
    public string Key { get; set; }

    [Required]
    [Column("value")]
    [StringLength(500)]
    public string Value { get; set; }

    [ForeignKey("VpdId")]
    [InverseProperty("MapVpdElements")]
    public virtual MapVpds Vpd { get; set; }
}
