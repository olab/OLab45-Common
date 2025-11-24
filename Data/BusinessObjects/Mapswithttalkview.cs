using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Keyless]
public partial class Mapswithttalkview
{
    [Column("QU_id")]
    public uint QuId { get; set; }

    [Column("QU_name")]
    [StringLength(50)]
    [MySqlCharSet("utf8mb3")]
    [MySqlCollation("utf8mb3_general_ci")]
    public string QuName { get; set; }

    [Column("entry_type_id")]
    public uint EntryTypeId { get; set; }

    [Column("MAP_number")]
    public uint MapNumber { get; set; }
}
