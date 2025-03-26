using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table( "oauth_providers" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class OauthProviders
{
  [Key]
  [Column( "id", TypeName = "int(10) unsigned" )]
  public uint Id { get; set; }

  [Required]
  [Column( "name" )]
  [StringLength( 250 )]
  public string Name { get; set; }

  [Required]
  [Column( "version" )]
  [StringLength( 200 )]
  public string Version { get; set; }

  [Column( "icon" )]
  [StringLength( 255 )]
  public string Icon { get; set; }

  [Column( "appId" )]
  [StringLength( 300 )]
  public string AppId { get; set; }

  [Column( "secret" )]
  [StringLength( 300 )]
  public string Secret { get; set; }
}
