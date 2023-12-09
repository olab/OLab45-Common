using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Data.BusinessObjects.API
{
    [Table("author_rights")]
    public partial class AuthorRights
    {
        [Key]
        [Column("id", TypeName = "int(10) unsigned")]
        public uint Id { get; set; }
        [Column("map_id", TypeName = "int(10) unsigned")]
        public uint MapId { get; set; }
        [Column("user_id", TypeName = "int(10) unsigned")]
        public uint UserId { get; set; }
    }
}
