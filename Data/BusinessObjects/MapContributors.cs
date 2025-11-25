using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_contributors")]
[Index("MapId", Name = "map_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapContributors
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("map_id")]
    public uint MapId { get; set; }

    [Column("role_id")]
    public uint RoleId { get; set; }

    [Required]
    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; }

    [Required]
    [Column("organization")]
    [StringLength(200)]
    public string Organization { get; set; }

    [Column("order")]
    public int Order { get; set; }

    [ForeignKey("MapId")]
    [InverseProperty("MapContributors")]
    public virtual Maps Map { get; set; }
}
