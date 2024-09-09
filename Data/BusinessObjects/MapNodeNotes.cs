using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_node_notes")]
[Index("MapNodeId", Name = "fk_map_node_id_idx")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapNodeNotes
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Column("map_node_id", TypeName = "int(10) unsigned")]
    public uint MapNodeId { get; set; }

    [Column("text", TypeName = "text")]
    public string Text { get; set; }

    [ForeignKey("MapNodeId")]
    [InverseProperty("MapNodeNotes")]
    public virtual MapNodes MapNode { get; set; }
}
