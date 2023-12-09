using Microsoft.EntityFrameworkCore;
using OLab.Api.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Data.BusinessObjects.API
{
    [Table("map_counter_common_rules")]
    [Index(nameof(MapId), Name = "map_id")]
    public partial class MapCounterCommonRules
    {
        public MapCounterCommonRules()
        {
            Cron = new HashSet<Cron>();
        }

        [Key]
        [Column("id", TypeName = "int(10) unsigned")]
        public uint Id { get; set; }
        [Column("map_id", TypeName = "int(10) unsigned")]
        public uint MapId { get; set; }
        [Required]
        [Column("rule")]
        public string Rule { get; set; }
        [Column("lightning", TypeName = "int(10)")]
        public int Lightning { get; set; }
        [Column("CorrectResponse")]
        public bool IsCorrect { get; set; }

        [ForeignKey(nameof(MapId))]
        [InverseProperty(nameof(Maps.MapCounterCommonRules))]
        public virtual Maps Map { get; set; }
        [InverseProperty("Rule")]
        public virtual ICollection<Cron> Cron { get; set; }
    }
}
