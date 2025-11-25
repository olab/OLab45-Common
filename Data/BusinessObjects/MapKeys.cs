using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_keys")]
[Index("Key", Name = "key")]
[Index("MapId", Name = "map_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapKeys
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("map_id")]
    public uint MapId { get; set; }

    [Required]
    [Column("key")]
    [StringLength(50)]
    public string Key { get; set; }

    [ForeignKey("MapId")]
    [InverseProperty("MapKeys")]
    public virtual Maps Map { get; set; }
}
