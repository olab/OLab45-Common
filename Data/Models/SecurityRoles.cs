using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Api.Models
{
  [Table("security_roles")]
  public partial class SecurityRoles
  {
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }
    [Required]
    [Column("name")]
    [StringLength(45)]
    public string Name { get; set; }
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
  }
}
