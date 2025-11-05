using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("webinar_poll")]
[Index("OnNode", Name = "on_node")]
[Index("ToNode", Name = "to_node")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class WebinarPoll
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("on_node")]
    public uint OnNode { get; set; }

    [Column("to_node")]
    public uint ToNode { get; set; }

    [Column("time")]
    public int Time { get; set; }

    [ForeignKey("OnNode")]
    [InverseProperty("WebinarPollOnNodeNavigation")]
    public virtual MapNodes OnNodeNavigation { get; set; }

    [ForeignKey("ToNode")]
    [InverseProperty("WebinarPollToNodeNavigation")]
    public virtual MapNodes ToNodeNavigation { get; set; }
}
