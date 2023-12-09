using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Data.BusinessObjects.API
{
    [Table("user_types")]
    [Index(nameof(Name), Name = "name")]
    public partial class UserTypes
    {
        [Key]
        [Column("id", TypeName = "int(10) unsigned")]
        public uint Id { get; set; }
        [Required]
        [Column("name")]
        [StringLength(30)]
        public string Name { get; set; }
        [Column("description")]
        [StringLength(100)]
        public string Description { get; set; }
    }
}
