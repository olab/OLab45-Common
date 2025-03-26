using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table( "webinar_node_poll" )]
[Index( "NodeId", Name = "node_id" )]
[Index( "WebinarId", Name = "webinar_id" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class WebinarNodePoll
{
  [Key]
  [Column( "id", TypeName = "int(10) unsigned" )]
  public uint Id { get; set; }

  [Column( "node_id", TypeName = "int(10) unsigned" )]
  public uint NodeId { get; set; }

  [Column( "webinar_id", TypeName = "int(10) unsigned" )]
  public uint WebinarId { get; set; }

  [Column( "time", TypeName = "int(11)" )]
  public int Time { get; set; }

  [ForeignKey( "NodeId" )]
  [InverseProperty( "WebinarNodePoll" )]
  public virtual MapNodes Node { get; set; }

  [ForeignKey( "WebinarId" )]
  [InverseProperty( "WebinarNodePoll" )]
  public virtual Webinars Webinar { get; set; }
}
