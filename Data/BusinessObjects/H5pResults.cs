using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("h5p_results")]
[Index("ContentId", "UserId", Name = "content_user")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class H5pResults
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("content_id")]
    public uint ContentId { get; set; }

    [Column("user_id")]
    public uint UserId { get; set; }

    [Column("score")]
    public uint Score { get; set; }

    [Column("max_score")]
    public uint MaxScore { get; set; }

    [Column("opened")]
    public uint Opened { get; set; }

    [Column("finished")]
    public uint Finished { get; set; }

    [Column("time")]
    public uint Time { get; set; }
}
