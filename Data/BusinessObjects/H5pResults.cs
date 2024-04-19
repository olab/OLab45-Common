﻿using System;
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
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Column("content_id", TypeName = "int(10) unsigned")]
    public uint ContentId { get; set; }

    [Column("user_id", TypeName = "int(10) unsigned")]
    public uint UserId { get; set; }

    [Column("score", TypeName = "int(10) unsigned")]
    public uint Score { get; set; }

    [Column("max_score", TypeName = "int(10) unsigned")]
    public uint MaxScore { get; set; }

    [Column("opened", TypeName = "int(10) unsigned")]
    public uint Opened { get; set; }

    [Column("finished", TypeName = "int(10) unsigned")]
    public uint Finished { get; set; }

    [Column("time", TypeName = "int(10) unsigned")]
    public uint Time { get; set; }
}
