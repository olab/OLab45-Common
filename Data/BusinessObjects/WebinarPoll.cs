using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table( "webinar_poll" )]
[Index( "OnNode", Name = "on_node" )]
[Index( "ToNode", Name = "to_node" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class WebinarPoll
{
  [Key]
  [Column( "id", TypeName = "int(10) unsigned" )]
  public uint Id { get; set; }

  [Column( "on_node", TypeName = "int(10) unsigned" )]
  public uint OnNode { get; set; }

  [Column( "to_node", TypeName = "int(10) unsigned" )]
  public uint ToNode { get; set; }

  [Column( "time", TypeName = "int(11)" )]
  public int Time { get; set; }

  [ForeignKey( "OnNode" )]
  [InverseProperty( "WebinarPollOnNodeNavigation" )]
  public virtual MapNodes OnNodeNavigation { get; set; }

  [ForeignKey( "ToNode" )]
  [InverseProperty( "WebinarPollToNodeNavigation" )]
  public virtual MapNodes ToNodeNavigation { get; set; }
}
