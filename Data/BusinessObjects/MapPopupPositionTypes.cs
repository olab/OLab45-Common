using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLabWebAPI.Model
{
    [Table("map_popup_position_types")]
    public partial class MapPopupPositionTypes
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
