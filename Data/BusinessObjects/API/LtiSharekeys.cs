using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Data.BusinessObjects.API
{
    [Table("lti_sharekeys")]
    public partial class LtiSharekeys
    {
        [Key]
        [Column("share_key_id")]
        [StringLength(32)]
        public string ShareKeyId { get; set; }
        [Required]
        [Column("primary_consumer_key")]
        [StringLength(255)]
        public string PrimaryConsumerKey { get; set; }
        [Required]
        [Column("primary_context_id")]
        [StringLength(255)]
        public string PrimaryContextId { get; set; }
        [Column("auto_approve")]
        public bool AutoApprove { get; set; }
        [Column("expires", TypeName = "datetime")]
        public DateTime Expires { get; set; }
    }
}
