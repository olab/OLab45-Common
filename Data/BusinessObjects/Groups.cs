using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("groups")]
[Index("Name", Name = "name")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class Groups
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Required]
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; }

    [Column("system", TypeName = "tinyint(1)")]
    public sbyte System { get; set; }

    [InverseProperty("Group")]
    public virtual ICollection<GrouproleAcls> GrouproleAcls { get; set; } = new List<GrouproleAcls>();

    [InverseProperty("Group")]
    public virtual ICollection<MapGrouproles> MapGrouproles { get; set; } = new List<MapGrouproles>();

    [InverseProperty("Group")]
    public virtual ICollection<MapGroups> MapGroups { get; set; } = new List<MapGroups>();

    [InverseProperty("Group")]
    public virtual ICollection<MapNodeGrouproles> MapNodeGrouproles { get; set; } = new List<MapNodeGrouproles>();

    [InverseProperty("Group")]
    public virtual ICollection<UserGrouproles> UserGrouproles { get; set; } = new List<UserGrouproles>();
}
