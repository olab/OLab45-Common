using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("usersessiontrace_counterupdate")]
[Index("CounterupdateId", Name = "stcu_fk_cu_idx")]
[Index("SessiontraceId", Name = "stcu_fk_st_idx")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class UsersessiontraceCounterupdate
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Column("sessiontrace_id", TypeName = "int(10) unsigned")]
    public uint SessiontraceId { get; set; }

    [Column("counterupdate_id", TypeName = "int(10) unsigned")]
    public uint CounterupdateId { get; set; }

    [ForeignKey("CounterupdateId")]
    [InverseProperty("UsersessiontraceCounterupdate")]
    public virtual UserCounterUpdate Counterupdate { get; set; }

    [ForeignKey("SessiontraceId")]
    [InverseProperty("UsersessiontraceCounterupdate")]
    public virtual UserSessiontraces Sessiontrace { get; set; }
}
