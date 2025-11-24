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
    [Column("id")]
    public uint Id { get; set; }

    [Column("description")]
    [StringLength(100)]
    public string Description { get; set; }

    [Required]
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; }

    [Column("is_system")]
    public sbyte? IsSystem { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<GrouproleAcls> GrouproleAcls { get; set; } = new List<GrouproleAcls>();

    [InverseProperty("Role")]
    public virtual ICollection<MapGrouproles> MapGrouproles { get; set; } = new List<MapGrouproles>();

    [InverseProperty("Role")]
    public virtual ICollection<MapNodeGrouproles> MapNodeGrouproles { get; set; } = new List<MapNodeGrouproles>();

    [InverseProperty("Role")]
    public virtual ICollection<UserGrouproles> UserGrouproles { get; set; } = new List<UserGrouproles>();
}
