using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_popup_positions")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapPopupPositions
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Required]
    [Column("title")]
    [StringLength(200)]
    public string Title { get; set; }
}
