using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table( "userresponse_counterupdate" )]
[Index( "CounterupdateId", Name = "urcu_fk_cu_idx" )]
[Index( "UserresponseId", Name = "urcu_fk_ur_idx" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class UserresponseCounterupdate
{
  [Key]
  [Column( "id", TypeName = "int(10) unsigned" )]
  public uint Id { get; set; }

  [Column( "userresponse_id", TypeName = "int(10) unsigned" )]
  public uint UserresponseId { get; set; }

  [Column( "counterupdate_id", TypeName = "int(10) unsigned" )]
  public uint CounterupdateId { get; set; }

  [ForeignKey( "CounterupdateId" )]
  [InverseProperty( "UserresponseCounterupdate" )]
  public virtual UserCounterUpdate Counterupdate { get; set; }

  [ForeignKey( "UserresponseId" )]
  [InverseProperty( "UserresponseCounterupdate" )]
  public virtual UserResponses Userresponse { get; set; }
}
