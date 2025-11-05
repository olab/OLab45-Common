using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[PrimaryKey("ContentId", "LibraryId", "DependencyType")]
[Table("h5p_contents_libraries")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class H5pContentsLibraries
{
    [Key]
    [Column("content_id")]
    public uint ContentId { get; set; }

    [Key]
    [Column("library_id")]
    public uint LibraryId { get; set; }

    [Key]
    [Column("dependency_type")]
    [StringLength(31)]
    [MySqlCharSet("utf8mb4")]
    [MySqlCollation("utf8mb4_unicode_ci")]
    public string DependencyType { get; set; }

    [Column("weight")]
    public ushort Weight { get; set; }

    [Column("drop_css")]
    public byte DropCss { get; set; }
}
