using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_elements_metadata")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapElementsMetadata
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Column("element_id", TypeName = "int(10) unsigned")]
    public uint ElementId { get; set; }

    [Column("description", TypeName = "text")]
    public string Description { get; set; }

    [Column("originURL")]
    [StringLength(300)]
    public string OriginUrl { get; set; }

    [Column("copyright", TypeName = "text")]
    public string Copyright { get; set; }
}
