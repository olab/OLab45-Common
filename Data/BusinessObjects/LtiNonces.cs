using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Api.Model
{
  [Table("lti_nonces")]
  public partial class LtiNonces
  {
    [Key]
    [Column("consumer_key")]
    public string ConsumerKey { get; set; }
    [Required]
    [Column("value")]
    [StringLength(32)]
    public string Value { get; set; }
    [Column("expires", TypeName = "datetime")]
    public DateTime Expires { get; set; }
  }
}
