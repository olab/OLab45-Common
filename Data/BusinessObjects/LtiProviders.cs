using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table( "lti_providers" )]
[Index( "Name", Name = "name", IsUnique = true )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class LtiProviders
{
  [Key]
  [Column( "id", TypeName = "int(10) unsigned" )]
  public uint Id { get; set; }

  [Column( "name", TypeName = "enum('video service')" )]
  public string Name { get; set; }

  [Column( "consumer_key" )]
  [StringLength( 255 )]
  public string ConsumerKey { get; set; }

  [Column( "secret" )]
  [StringLength( 32 )]
  public string Secret { get; set; }

  [Column( "launch_url" )]
  [StringLength( 255 )]
  public string LaunchUrl { get; set; }
}
