﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_node_sections")]
[Index("MapId", Name = "map_id")]
public partial class MapNodeSections
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Required]
    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; }

    [Column("map_id", TypeName = "int(10) unsigned")]
    public uint MapId { get; set; }

    [Required]
    [Column("orderBy", TypeName = "enum('random','x','y')")]
    public string OrderBy { get; set; }

    [ForeignKey("MapId")]
    [InverseProperty("MapNodeSections")]
    public virtual Maps Map { get; set; }

    [InverseProperty("Section")]
    public virtual ICollection<MapNodeSectionNodes> MapNodeSectionNodes { get; } = new List<MapNodeSectionNodes>();
}
