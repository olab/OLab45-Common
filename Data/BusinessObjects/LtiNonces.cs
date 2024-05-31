using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table("lti_nonces")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
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
