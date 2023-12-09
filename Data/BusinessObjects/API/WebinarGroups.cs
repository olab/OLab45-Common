using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Data.BusinessObjects.API
{
    [Table("webinar_groups")]
    public partial class WebinarGroups
    {
        [Key]
        [Column("id", TypeName = "int(10) unsigned")]
        public uint Id { get; set; }
        [Column("webinar_id", TypeName = "int(10) unsigned")]
        public uint WebinarId { get; set; }
        [Column("group_id", TypeName = "int(10) unsigned")]
        public uint GroupId { get; set; }
    }
}
