using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_keys")]
[Index("Key", Name = "key")]
[Index("MapId", Name = "map_id")]
public partial class MapKeys
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Column("map_id", TypeName = "int(10) unsigned")]
    public uint MapId { get; set; }

    [Required]
    [Column("key")]
    [StringLength(50)]
    public string Key { get; set; }

    [ForeignKey("MapId")]
    [InverseProperty("MapKeys")]
    public virtual Maps Map { get; set; }
}
