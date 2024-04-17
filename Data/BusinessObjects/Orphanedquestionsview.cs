using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Keyless]
public partial class Orphanedquestionsview
{
    [Column("QuID", TypeName = "int(10) unsigned")]
    public uint QuId { get; set; }

    [StringLength(50)]
    [MySqlCharSet("utf8mb3")]
    [MySqlCollation("utf8mb3_general_ci")]
    public string QuName { get; set; }

    [Required]
    [StringLength(45)]
    [MySqlCharSet("utf8mb3")]
    [MySqlCollation("utf8mb3_general_ci")]
    public string QuScope { get; set; }

    [Column("QuMapID", TypeName = "int(10) unsigned")]
    public uint QuMapId { get; set; }

    [Column("mapName")]
    [StringLength(200)]
    [MySqlCharSet("utf8mb3")]
    [MySqlCollation("utf8mb3_general_ci")]
    public string MapName { get; set; }

    [Column("MapID", TypeName = "int(10) unsigned")]
    public uint? MapId { get; set; }
}
