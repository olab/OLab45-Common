using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[PrimaryKey("ContentId", "TagId")]
[Table("h5p_contents_tags")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class H5pContentsTags
{
    [Key]
    [Column("content_id")]
    public uint ContentId { get; set; }

    [Key]
    [Column("tag_id")]
    public uint TagId { get; set; }
}
