using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Data.BusinessObjects
{
    [Table("map_popup_assign_types")]
    public partial class MapPopupAssignTypes
    {
        [Key]
        [Column("id", TypeName = "int(10) unsigned")]
        public uint Id { get; set; }
        [Required]
        [Column("title")]
        [StringLength(200)]
        public string Title { get; set; }
    }
}
