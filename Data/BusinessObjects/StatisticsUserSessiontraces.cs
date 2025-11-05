using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("statistics_user_sessiontraces")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class StatisticsUserSessiontraces
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("session_id")]
    public uint SessionId { get; set; }

    [Column("user_id")]
    public uint UserId { get; set; }

    [Column("map_id")]
    public uint MapId { get; set; }

    [Column("node_id")]
    public uint NodeId { get; set; }

    [Column("is_redirected")]
    public bool IsRedirected { get; set; }

    [Column("counters")]
    [StringLength(700)]
    public string Counters { get; set; }

    [Column("date_stamp")]
    [Precision(18, 6)]
    public decimal? DateStamp { get; set; }

    [Column("confidence")]
    public short? Confidence { get; set; }

    [Column("dams")]
    [StringLength(700)]
    public string Dams { get; set; }

    [Column("bookmark_made")]
    [Precision(18, 6)]
    public decimal? BookmarkMade { get; set; }

    [Column("bookmark_used")]
    [Precision(18, 6)]
    public decimal? BookmarkUsed { get; set; }

    [Column("end_date_stamp", TypeName = "decimal(18,6) unsigned")]
    public decimal? EndDateStamp { get; set; }
}
