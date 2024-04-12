using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("user_types")]
[Index("Name", Name = "name")]
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
