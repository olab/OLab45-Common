using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Keyless]
public partial class Orphanedconstantsview
{
    [Column("CoID", TypeName = "int(10) unsigned")]
    public uint CoId { get; set; }

    [StringLength(45)]
    [MySqlCharSet("utf8mb3")]
    [MySqlCollation("utf8mb3_general_ci")]
    public string CoName { get; set; }

    [Required]
    [StringLength(45)]
    [MySqlCharSet("utf8mb3")]
    [MySqlCollation("utf8mb3_general_ci")]
    public string CoScope { get; set; }

    [Column("CoMapID", TypeName = "int(10) unsigned")]
    public uint CoMapId { get; set; }

    [Column("mapName")]
    [StringLength(200)]
    [MySqlCharSet("utf8mb3")]
    [MySqlCollation("utf8mb3_general_ci")]
    public string MapName { get; set; }

    [Column("MapID", TypeName = "int(10) unsigned")]
    public uint? MapId { get; set; }
}
