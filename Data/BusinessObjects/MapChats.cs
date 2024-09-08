using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_chats")]
[Index("CounterId", Name = "counter_id")]
[Index("MapId", "CounterId", Name = "map_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapChats
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Column("map_id", TypeName = "int(10) unsigned")]
    public uint MapId { get; set; }

    [Column("counter_id", TypeName = "int(10) unsigned")]
    public uint? CounterId { get; set; }

    [Required]
    [Column("stem", TypeName = "text")]
    public string Stem { get; set; }

    [Column("is_private", TypeName = "int(4)")]
    public int IsPrivate { get; set; }

    [ForeignKey("MapId")]
    [InverseProperty("MapChats")]
    public virtual Maps Map { get; set; }

    [InverseProperty("Chat")]
    public virtual ICollection<MapChatElements> MapChatElements { get; } = new List<MapChatElements>();
}
