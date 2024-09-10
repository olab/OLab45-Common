using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("system_settings")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class SystemSettings
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Required]
    [Column("key")]
    [StringLength(45)]
    public string Key { get; set; }

    [Column("description")]
    [StringLength(256)]
    public string Description { get; set; }

    [Required]
    [Column("value")]
    [StringLength(256)]
    public string Value { get; set; }

    [Column("system_settingscol")]
    [StringLength(45)]
    public string SystemSettingscol { get; set; }
}
