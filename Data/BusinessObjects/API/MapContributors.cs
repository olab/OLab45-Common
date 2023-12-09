using Microsoft.EntityFrameworkCore;
using OLab.Api.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Data.BusinessObjects.API
{
    [Table("map_contributors")]
    [Index(nameof(MapId), Name = "map_id")]
    public partial class MapContributors
    {
        [Key]
        [Column("id", TypeName = "int(10) unsigned")]
        public uint Id { get; set; }
        [Column("map_id", TypeName = "int(10) unsigned")]
        public uint MapId { get; set; }
        [Column("role_id", TypeName = "int(10) unsigned")]
        public uint RoleId { get; set; }
        [Required]
        [Column("name")]
        [StringLength(200)]
        public string Name { get; set; }
        [Required]
        [Column("organization")]
        [StringLength(200)]
        public string Organization { get; set; }
        [Column("order", TypeName = "int(10)")]
        public int Order { get; set; }

        [ForeignKey(nameof(MapId))]
        [InverseProperty(nameof(Maps.MapContributors))]
        public virtual Maps Map { get; set; }
    }
}
