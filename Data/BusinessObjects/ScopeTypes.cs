using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Data.BusinessObjects
{
    [Table("scope_types")]
    public partial class ScopeTypes
    {
        [Key]
        [Column("id", TypeName = "int(10) unsigned")]
        public uint Id { get; set; }
        [Required]
        [Column("name")]
        [StringLength(45)]
        public string Name { get; set; }
        [Column("description")]
        [StringLength(45)]
        public string Description { get; set; }
    }
}
