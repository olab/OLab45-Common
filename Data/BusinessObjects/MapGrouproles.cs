using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_grouproles")]
[Index("GroupId", Name = "group_id")]
[Index("MapId", Name = "mgr_ibfk_node_idx")]
[Index("RoleId", Name = "role_id")]
public partial class MapGrouproles
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Column("map_id", TypeName = "int(10) unsigned")]
    public uint MapId { get; set; }

    [Column("group_id", TypeName = "int(10) unsigned")]
    public uint? GroupId { get; set; }

    [Column("role_id", TypeName = "int(10) unsigned")]
    public uint? RoleId { get; set; }

    [ForeignKey("GroupId")]
    [InverseProperty("MapGrouproles")]
    public virtual Groups Group { get; set; }

    [ForeignKey("MapId")]
    [InverseProperty("MapGrouproles")]
    public virtual Maps Map { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("MapGrouproles")]
    public virtual Roles Role { get; set; }
}
