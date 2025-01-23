using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table( "lti_sharekeys" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class LtiSharekeys
{
  [Key]
  [Column( "share_key_id" )]
  [StringLength( 32 )]
  public string ShareKeyId { get; set; }

  [Required]
  [Column( "primary_consumer_key" )]
  [StringLength( 255 )]
  public string PrimaryConsumerKey { get; set; }

  [Required]
  [Column( "primary_context_id" )]
  [StringLength( 255 )]
  public string PrimaryContextId { get; set; }

  [Column( "auto_approve" )]
  public bool AutoApprove { get; set; }

  [Column( "expires", TypeName = "datetime" )]
  public DateTime Expires { get; set; }
}
