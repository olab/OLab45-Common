using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Model
{
  [Table("map_users")]
  [Index(nameof(MapId), Name = "map_id")]
  [Index(nameof(UserId), Name = "user_id")]
  public partial class MapUsers
  {
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }
    [Column("map_id", TypeName = "int(10) unsigned")]
    public uint MapId { get; set; }
    [Column("user_id", TypeName = "int(10) unsigned")]
    public uint UserId { get; set; }

    [ForeignKey(nameof(MapId))]
    [InverseProperty(nameof(Maps.MapUsers))]
    public virtual Maps Map { get; set; }
    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(Users.MapUsers))]
    public virtual Users User { get; set; }
  }
}
