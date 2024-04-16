using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("webinar_macros")]
[Index("WebinarId", Name = "webinar_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class WebinarMacros
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Column("text")]
    [StringLength(255)]
    public string Text { get; set; }

    [Column("hot_keys")]
    [StringLength(255)]
    public string HotKeys { get; set; }

    [Column("webinar_id", TypeName = "int(10) unsigned")]
    public uint WebinarId { get; set; }
}
