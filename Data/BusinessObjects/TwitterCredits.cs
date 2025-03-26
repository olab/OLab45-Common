using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table( "twitter_credits" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class TwitterCredits
{
  [Key]
  [Column( "id", TypeName = "int(10) unsigned" )]
  public uint Id { get; set; }

  [Required]
  [Column( "API_key", TypeName = "text" )]
  public string ApiKey { get; set; }

  [Required]
  [Column( "API_secret", TypeName = "text" )]
  public string ApiSecret { get; set; }

  [Required]
  [Column( "Access_token", TypeName = "text" )]
  public string AccessToken { get; set; }

  [Required]
  [Column( "Access_token_secret", TypeName = "text" )]
  public string AccessTokenSecret { get; set; }
}
