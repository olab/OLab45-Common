using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_node_grouproles")]
[Index("GroupId", Name = "ifk_mngr_group_idx")]
[Index("NodeId", Name = "ifk_mngr_node_idx")]
[Index("RoleId", Name = "ifk_mngr_role_idx")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapNodeGrouproles
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Column("node_id", TypeName = "int(10) unsigned")]
    public uint NodeId { get; set; }

    [Column("group_id", TypeName = "int(10) unsigned")]
    public uint GroupId { get; set; }

    [Column("role_id", TypeName = "int(10) unsigned")]
    public uint RoleId { get; set; }

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
