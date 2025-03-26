using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table( "user_counter_update" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class UserCounterUpdate
{
  [Key]
  [Column( "id", TypeName = "int(10) unsigned" )]
  public uint Id { get; set; }

  [Required]
  [Column( "counter_state" )]
  [StringLength( 8192 )]
  public string CounterState { get; set; }

  [InverseProperty( "Counterupdate" )]
  public virtual ICollection<UserresponseCounterupdate> UserresponseCounterupdate { get; set; } = new List<UserresponseCounterupdate>();

  [InverseProperty( "Counterupdate" )]
  public virtual ICollection<UsersessiontraceCounterupdate> UsersessiontraceCounterupdate { get; set; } = new List<UsersessiontraceCounterupdate>();
}
