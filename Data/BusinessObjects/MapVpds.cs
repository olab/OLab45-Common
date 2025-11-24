using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_vpds")]
[Index("VpdTypeId", Name = "vpd_type_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapVpds
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("map_id")]
    public uint MapId { get; set; }

    [Column("vpd_type_id")]
    public uint VpdTypeId { get; set; }

    [InverseProperty("Vpd")]
    public virtual ICollection<MapVpdElements> MapVpdElements { get; set; } = new List<MapVpdElements>();

    [ForeignKey("VpdTypeId")]
    [InverseProperty("MapVpds")]
    public virtual MapVpdTypes VpdType { get; set; }
}
