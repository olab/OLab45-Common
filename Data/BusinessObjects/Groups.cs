using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("groups")]
[Index("Name", Name = "name")]
public partial class Groups
{
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }

    [Column("description")]
    [StringLength(100)]
    public string Description { get; set; }

    [Required]
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; }

    [InverseProperty("Group")]
    public virtual ICollection<MapGroups> MapGroups { get; } = new List<MapGroups>();

    [InverseProperty("Group")]
    public virtual ICollection<SecurityRoles> SecurityRoles { get; } = new List<SecurityRoles>();

    [InverseProperty("Group")]
    public virtual ICollection<UserGroups> UserGroups { get; } = new List<UserGroups>();
}
