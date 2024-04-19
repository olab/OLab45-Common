using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("user_counter_update")]
public partial class UserCounterUpdate
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Required]
    [Column("counter_state")]
    [StringLength(8192)]
    public string CounterState { get; set; }

    [InverseProperty("Counterupdate")]
    public virtual ICollection<UserresponseCounterupdate> UserresponseCounterupdate { get; } = new List<UserresponseCounterupdate>();

    [InverseProperty("Counterupdate")]
    public virtual ICollection<UsersessiontraceCounterupdate> UsersessiontraceCounterupdate { get; } = new List<UsersessiontraceCounterupdate>();
}
