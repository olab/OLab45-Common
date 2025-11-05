using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("system_files")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class SystemFiles
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Required]
    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; }

    [Column("description", TypeName = "text")]
    public string Description { get; set; }

    [Column("mime")]
    [StringLength(500)]
    public string Mime { get; set; }

    [Required]
    [Column("path")]
    [StringLength(300)]
    public string Path { get; set; }

    [Column("type")]
    public int? Type { get; set; }

    [Column("args")]
    [StringLength(100)]
    public string Args { get; set; }

    [Column("width")]
    public int? Width { get; set; }

    [Required]
    [Column("width_type")]
    [StringLength(2)]
    public string WidthType { get; set; }

    [Column("height")]
    public int? Height { get; set; }

    [Required]
    [Column("height_type")]
    [StringLength(2)]
    public string HeightType { get; set; }

    [Column("h_align")]
    [StringLength(20)]
    public string HAlign { get; set; }

    [Column("v_align")]
    [StringLength(20)]
    public string VAlign { get; set; }

    [Column("origin_url")]
    [StringLength(512)]
    public string OriginUrl { get; set; }

    [Column("copyright")]
    [StringLength(45)]
    public string Copyright { get; set; }

    [Column("file_size")]
    public int? FileSize { get; set; }

    [Column("is_shared")]
    public sbyte IsShared { get; set; }

    [Column("is_private")]
    public sbyte IsPrivate { get; set; }

    [Column("is_embedded")]
    public sbyte? IsEmbedded { get; set; }

    [Column("is_media")]
    public sbyte? IsMedia { get; set; }

    [Column("encoded_content", TypeName = "blob")]
    public byte[] EncodedContent { get; set; }

    [Column("imageable_id")]
    public uint ImageableId { get; set; }

    [Required]
    [Column("imageable_type")]
    [StringLength(45)]
    public string ImageableType { get; set; }

    [Column("is_system")]
    public int? IsSystem { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }
}
