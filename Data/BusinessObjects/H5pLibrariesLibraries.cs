using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[PrimaryKey("LibraryId", "RequiredLibraryId")]
[Table("h5p_libraries_libraries")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class H5pLibrariesLibraries
{
    [Key]
    [Column("library_id", TypeName = "int(10) unsigned")]
    public uint LibraryId { get; set; }

    [Key]
    [Column("required_library_id", TypeName = "int(10) unsigned")]
    public uint RequiredLibraryId { get; set; }

    [Required]
    [Column("dependency_type")]
    [StringLength(31)]
    [MySqlCharSet("utf8mb4")]
    [MySqlCollation("utf8mb4_unicode_ci")]
    public string DependencyType { get; set; }
}
