using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table( "statements" )]
[Index( "SessionId", Name = "session_id" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class Statements
{
  [Key]
  [Column( "id", TypeName = "int(10) unsigned" )]
  public uint Id { get; set; }

  [Column( "session_id", TypeName = "int(10) unsigned" )]
  public uint? SessionId { get; set; }

  [Column( "initiator", TypeName = "tinyint(3) unsigned" )]
  public byte Initiator { get; set; }

  [Required]
  [Column( "statement", TypeName = "text" )]
  public string Statement { get; set; }

  [Column( "timestamp" )]
  [Precision( 18, 6 )]
  public decimal Timestamp { get; set; }

  [Column( "created_at", TypeName = "datetime" )]
  public DateTime CreatedAt { get; set; }

  [Column( "updated_at", TypeName = "datetime" )]
  public DateTime UpdatedAt { get; set; }

  [InverseProperty( "Statement" )]
  public virtual ICollection<LrsStatement> LrsStatement { get; } = new List<LrsStatement>();

  [ForeignKey( "SessionId" )]
  [InverseProperty( "Statements" )]
  public virtual UserSessions Session { get; set; }
}
