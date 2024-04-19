using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[PrimaryKey("Type", "LibraryName", "LibraryVersion")]
[Table("h5p_counters")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class H5pCounters
{
    [Key]
    [Column("type")]
    [StringLength(63)]
    [MySqlCharSet("utf8mb4")]
    [MySqlCollation("utf8mb4_unicode_ci")]
    public string Type { get; set; }

    [Key]
    [Column("library_name")]
    [StringLength(127)]
    [MySqlCharSet("utf8mb4")]
    [MySqlCollation("utf8mb4_unicode_ci")]
    public string LibraryName { get; set; }

    [Key]
    [Column("library_version")]
    [StringLength(31)]
    [MySqlCharSet("utf8mb4")]
    [MySqlCollation("utf8mb4_unicode_ci")]
    public string LibraryVersion { get; set; }

    [Column("num", TypeName = "int(10) unsigned")]
    public uint Num { get; set; }
}
