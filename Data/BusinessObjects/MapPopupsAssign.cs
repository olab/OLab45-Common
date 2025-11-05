using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_popups_assign")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapPopupsAssign
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("map_popup_id")]
    public uint MapPopupId { get; set; }

    [Column("assign_type_id")]
    public uint AssignTypeId { get; set; }

    [Column("assign_to_id")]
    public uint AssignToId { get; set; }

    [Column("redirect_to_id")]
    public uint? RedirectToId { get; set; }

    [Column("redirect_type_id")]
    public uint RedirectTypeId { get; set; }
}
