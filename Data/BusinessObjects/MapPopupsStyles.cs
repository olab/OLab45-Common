using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_popups_styles")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapPopupsStyles
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("map_popup_id")]
    public uint MapPopupId { get; set; }

    [Column("is_default_background_color")]
    public sbyte IsDefaultBackgroundColor { get; set; }

    [Column("is_background_transparent")]
    public sbyte IsBackgroundTransparent { get; set; }

    [Column("background_color")]
    [StringLength(10)]
    public string BackgroundColor { get; set; }

    [Column("font_color")]
    [StringLength(10)]
    public string FontColor { get; set; }

    [Column("border_color")]
    [StringLength(10)]
    public string BorderColor { get; set; }

    [Column("is_border_transparent")]
    public sbyte IsBorderTransparent { get; set; }

    [Required]
    [Column("background_transparent")]
    [StringLength(4)]
    public string BackgroundTransparent { get; set; }

    [Required]
    [Column("border_transparent")]
    [StringLength(4)]
    public string BorderTransparent { get; set; }
}
