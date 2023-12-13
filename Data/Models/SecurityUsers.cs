using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Api.Models
{
  [Table("security_users")]
  [Index(nameof(UserId), Name = "fk_security_users_user")]
  public partial class SecurityUsers
  {
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }
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
    [Column("user_id", TypeName = "int(10) unsigned")]
    public uint UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(Users.SecurityUsers))]
    public virtual Users User { get; set; }

    [Column("iss")]
    public string Issuer { get; set; }
  }
}
