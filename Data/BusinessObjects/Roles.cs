using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("roles")]
[Index("Name", Name = "name")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class Roles
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

    [InverseProperty("Role")]
    public virtual ICollection<SecurityRoles> SecurityRoles { get; } = new List<SecurityRoles>();

    [InverseProperty("Role")]
    public virtual ICollection<UserGroups> UserGroups { get; } = new List<UserGroups>();
}
