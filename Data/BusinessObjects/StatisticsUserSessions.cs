using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("statistics_user_sessions")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class StatisticsUserSessions
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("user_id")]
    public uint UserId { get; set; }

    [Column("map_id")]
    public uint MapId { get; set; }

    [Column("start_time")]
    [Precision(18, 6)]
    public decimal StartTime { get; set; }

    [Column("end_time")]
    [Precision(18, 6)]
    public decimal? EndTime { get; set; }

    [Required]
    [Column("user_ip")]
    [StringLength(50)]
    public string UserIp { get; set; }

    [Column("webinar_id")]
    public uint WebinarId { get; set; }

    [Column("webinar_step")]
    public int WebinarStep { get; set; }

    [Column("date_save_id")]
    public uint DateSaveId { get; set; }
}
