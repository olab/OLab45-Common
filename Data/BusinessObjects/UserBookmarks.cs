using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("user_bookmarks")]
[Index("NodeId", Name = "node_id")]
[Index("SessionId", Name = "session_id")]
[Index("UserId", Name = "user_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class UserBookmarks
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("session_id")]
    public uint SessionId { get; set; }

    [Column("node_id")]
    public uint NodeId { get; set; }

    [Column("user_id")]
    public uint UserId { get; set; }

    [ForeignKey("NodeId")]
    [InverseProperty("UserBookmarks")]
    public virtual MapNodes Node { get; set; }

    [ForeignKey("SessionId")]
    [InverseProperty("UserBookmarks")]
    public virtual UserSessions Session { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserBookmarks")]
    public virtual Users User { get; set; }
}
