using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[PrimaryKey("ContentId", "UserId", "SubContentId", "DataId")]
[Table("h5p_contents_user_data")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class H5pContentsUserData
{
    [Key]
    [Column("content_id")]
    public uint ContentId { get; set; }

    [Key]
    [Column("user_id")]
    public uint UserId { get; set; }

    [Key]
    [Column("sub_content_id")]
    public uint SubContentId { get; set; }

    [Key]
    [Column("data_id")]
    [StringLength(127)]
    [MySqlCharSet("utf8mb4")]
    [MySqlCollation("utf8mb4_unicode_ci")]
    public string DataId { get; set; }

    [Required]
    [Column("data")]
    [MySqlCharSet("utf8mb4")]
    [MySqlCollation("utf8mb4_unicode_ci")]
    public string Data { get; set; }

    [Column("preload")]
    public byte Preload { get; set; }

    [Column("invalidate")]
    public byte Invalidate { get; set; }

    [Column("updated_at", TypeName = "timestamp")]
    public DateTime UpdatedAt { get; set; }
}
