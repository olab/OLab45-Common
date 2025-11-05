using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("user_sessiontraces")]
[Index("MapId", Name = "map_id")]
[Index("NodeId", Name = "node_id")]
[Index("SessionId", Name = "session_id")]
[Index("UserId", Name = "user_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class UserSessiontraces
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
    [StringLength(2000)]
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

    [Column("end_date_stamp")]
    [Precision(18, 6)]
    public decimal? EndDateStamp { get; set; }

    [ForeignKey("MapId")]
    [InverseProperty("UserSessiontraces")]
    public virtual Maps Map { get; set; }

    [ForeignKey("NodeId")]
    [InverseProperty("UserSessiontraces")]
    public virtual MapNodes Node { get; set; }

    [ForeignKey("SessionId")]
    [InverseProperty("UserSessiontraces")]
    public virtual UserSessions Session { get; set; }

    [InverseProperty("Sessiontrace")]
    public virtual ICollection<UsersessiontraceCounterupdate> UsersessiontraceCounterupdate { get; set; } = new List<UsersessiontraceCounterupdate>();
}
