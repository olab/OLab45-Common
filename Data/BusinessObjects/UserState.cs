using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("user_state")]
[Index("MapId", Name = "map_fk_idx")]
[Index("MapNodeId", Name = "map_node_fk_idx")]
[Index("UserId", Name = "user_fk_idx")]
public partial class UserState
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Column("user_id", TypeName = "int(10) unsigned")]
    public uint UserId { get; set; }

    [Column("map_id", TypeName = "int(10) unsigned")]
    public uint MapId { get; set; }

    [Column("map_node_id", TypeName = "int(10) unsigned")]
    public uint MapNodeId { get; set; }

    [Required]
    [Column("state_data", TypeName = "blob")]
    public byte[] StateData { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column("session_id", TypeName = "int(10) unsigned")]
    public uint SessionId { get; set; }

    [ForeignKey("MapId")]
    [InverseProperty("UserState")]
    public virtual Maps Map { get; set; }

    [ForeignKey("MapNodeId")]
    [InverseProperty("UserState")]
    public virtual MapNodes MapNode { get; set; }
}
