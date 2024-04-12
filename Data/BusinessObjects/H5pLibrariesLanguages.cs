using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[PrimaryKey("LibraryId", "LanguageCode")]
[Table("h5p_libraries_languages")]
public partial class H5pLibrariesLanguages
{
    [Key]
    [Column("library_id", TypeName = "int(10) unsigned")]
    public uint LibraryId { get; set; }

    [Key]
    [Column("language_code")]
    [StringLength(31)]
    [MySqlCharSet("utf8mb4")]
    [MySqlCollation("utf8mb4_unicode_ci")]
    public string LanguageCode { get; set; }

    [Required]
    [Column("translation", TypeName = "text")]
    [MySqlCharSet("utf8mb4")]
    [MySqlCollation("utf8mb4_unicode_ci")]
    public string Translation { get; set; }
}
