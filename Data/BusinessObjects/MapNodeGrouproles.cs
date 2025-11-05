using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_node_grouproles")]
[Index("GroupId", Name = "group_id")]
[Index("NodeId", Name = "mngr_ibfk_node_idx")]
[Index("RoleId", Name = "role_id")]
[MySqlCollation("utf8mb4_general_ci")]
public partial class MapNodeGrouproles
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("node_id")]
    public uint NodeId { get; set; }

    [Column("group_id")]
    public uint? GroupId { get; set; }

    [Column("role_id")]
    public uint? RoleId { get; set; }

    [ForeignKey("GroupId")]
    [InverseProperty("MapNodeGrouproles")]
    public virtual Groups Group { get; set; }

    [ForeignKey("NodeId")]
    [InverseProperty("MapNodeGrouproles")]
    public virtual MapNodes Node { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("MapNodeGrouproles")]
    public virtual Roles Role { get; set; }
}
