using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_node_section_nodes")]
[Index("NodeId", Name = "node_id")]
[Index("SectionId", Name = "section_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapNodeSectionNodes
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Column("section_id", TypeName = "int(10) unsigned")]
    public uint SectionId { get; set; }

    [Column("node_id", TypeName = "int(10) unsigned")]
    public uint NodeId { get; set; }

    [Column("order", TypeName = "int(10)")]
    public int Order { get; set; }

    [Required]
    [Column("node_type", TypeName = "enum('regular','in','out','crucial')")]
    public string NodeType { get; set; }

    [ForeignKey("NodeId")]
    [InverseProperty("MapNodeSectionNodes")]
    public virtual MapNodes Node { get; set; }

    [ForeignKey("SectionId")]
    [InverseProperty("MapNodeSectionNodes")]
    public virtual MapNodeSections Section { get; set; }
}
