using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Data.BusinessObjects
{
    [Table("ttalk_participant")]
    public partial class TTalkParticipant
    {
        [Key]
        [Column("id", TypeName = "int(10) unsigned")]
        public uint Id { get; set; }
        [Required]
        [Column("user_name")]
        [StringLength(255)]
        public string UserName { get; set; }
        [Required]
        [Column("user_issuer")]
        [StringLength(45)]
        public string UserIssuer { get; set; }
        [Required]
        [Column("connection_id")]
        [StringLength(45)]
        public string ConnectionId { get; set; }
        [Column("start", TypeName = "datetime")]
        public DateTime Start { get; set; }
        [Column("end", TypeName = "datetime")]
        public DateTime? End { get; set; }
        [Required]
        [Column("room_name")]
        [StringLength(255)]
        public string RoomName { get; set; }
        [Column("room_index", TypeName = "int(10) unsigned")]
        public int? RoomIndex { get; set; }
        [Column("is_moderator", TypeName = "tinyint(4)")]
        public sbyte IsModerator { get; set; }
    }
}
