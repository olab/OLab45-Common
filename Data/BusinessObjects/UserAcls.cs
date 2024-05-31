using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table("user_acls")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class UserAcls
{
  [Key]
  [Column("id", TypeName = "int(10) unsigned")]
  public uint Id { get; set; }

  [Column("user_id", TypeName = "int(10) unsigned")]
  public uint UserId { get; set; }

  [Column("iss")]
  [StringLength(45)]
  public string Iss { get; set; }

  [Column("imageable_id", TypeName = "int(10) unsigned")]
  public uint ImageableId { get; set; }

  [Required]
  [Column("imageable_type")]
  [StringLength(45)]
  public string ImageableType { get; set; }

  [Required]
  [Column("acl")]
  [StringLength(45)]
  public string Acl { get; set; }

  [Column("acl2", TypeName = "bit(3)")]
  public ulong Acl2 { get; set; }
}
