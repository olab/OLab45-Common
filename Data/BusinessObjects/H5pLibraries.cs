using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("h5p_libraries")]
[Index("Name", "MajorVersion", "MinorVersion", "PatchVersion", Name = "name_version")]
[Index("Runnable", Name = "runnable")]
public partial class H5pLibraries
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Column("created_at", TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp")]
    public DateTime UpdatedAt { get; set; }

    [Required]
    [Column("name")]
    [StringLength(127)]
    [MySqlCharSet("utf8mb4")]
    [MySqlCollation("utf8mb4_unicode_ci")]
    public string Name { get; set; }

    [Required]
    [Column("title")]
    [StringLength(255)]
    [MySqlCharSet("utf8mb4")]
    [MySqlCollation("utf8mb4_unicode_ci")]
    public string Title { get; set; }

    [Column("major_version", TypeName = "int(10) unsigned")]
    public uint MajorVersion { get; set; }

    [Column("minor_version", TypeName = "int(10) unsigned")]
    public uint MinorVersion { get; set; }

    [Column("patch_version", TypeName = "int(10) unsigned")]
    public uint PatchVersion { get; set; }

    [Column("runnable", TypeName = "int(10) unsigned")]
    public uint Runnable { get; set; }

    [Column("restricted", TypeName = "int(10) unsigned")]
    public uint Restricted { get; set; }

    [Column("fullscreen", TypeName = "int(10) unsigned")]
    public uint Fullscreen { get; set; }

    [Required]
    [Column("embed_types")]
    [StringLength(255)]
    [MySqlCharSet("utf8mb4")]
    [MySqlCollation("utf8mb4_unicode_ci")]
    public string EmbedTypes { get; set; }

    [Column("preloaded_js", TypeName = "text")]
    [MySqlCharSet("utf8mb4")]
    [MySqlCollation("utf8mb4_unicode_ci")]
    public string PreloadedJs { get; set; }

    [Column("preloaded_css", TypeName = "text")]
    [MySqlCharSet("utf8mb4")]
    [MySqlCollation("utf8mb4_unicode_ci")]
    public string PreloadedCss { get; set; }

    [Column("drop_library_css", TypeName = "text")]
    [MySqlCharSet("utf8mb4")]
    [MySqlCollation("utf8mb4_unicode_ci")]
    public string DropLibraryCss { get; set; }

    [Required]
    [Column("semantics", TypeName = "text")]
    [MySqlCharSet("utf8mb4")]
    [MySqlCollation("utf8mb4_unicode_ci")]
    public string Semantics { get; set; }

    [Required]
    [Column("tutorial_url")]
    [StringLength(1023)]
    [MySqlCharSet("utf8mb4")]
    [MySqlCollation("utf8mb4_unicode_ci")]
    public string TutorialUrl { get; set; }
}
