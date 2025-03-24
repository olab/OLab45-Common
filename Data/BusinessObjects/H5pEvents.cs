using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table( "h5p_events" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class H5pEvents
{
  [Key]
  [Column( "id", TypeName = "int(10) unsigned" )]
  public uint Id { get; set; }

  [Column( "user_id", TypeName = "int(10) unsigned" )]
  public uint UserId { get; set; }

  [Column( "created_at", TypeName = "int(10) unsigned" )]
  public uint CreatedAt { get; set; }

  [Required]
  [Column( "type" )]
  [StringLength( 63 )]
  [MySqlCharSet( "utf8mb4" )]
  [MySqlCollation( "utf8mb4_unicode_ci" )]
  public string Type { get; set; }

  [Required]
  [Column( "sub_type" )]
  [StringLength( 63 )]
  [MySqlCharSet( "utf8mb4" )]
  [MySqlCollation( "utf8mb4_unicode_ci" )]
  public string SubType { get; set; }

  [Column( "content_id", TypeName = "int(10) unsigned" )]
  public uint ContentId { get; set; }

  [Required]
  [Column( "content_title" )]
  [StringLength( 255 )]
  [MySqlCharSet( "utf8mb4" )]
  [MySqlCollation( "utf8mb4_unicode_ci" )]
  public string ContentTitle { get; set; }

  [Required]
  [Column( "library_name" )]
  [StringLength( 127 )]
  [MySqlCharSet( "utf8mb4" )]
  [MySqlCollation( "utf8mb4_unicode_ci" )]
  public string LibraryName { get; set; }

  [Required]
  [Column( "library_version" )]
  [StringLength( 31 )]
  [MySqlCharSet( "utf8mb4" )]
  [MySqlCollation( "utf8mb4_unicode_ci" )]
  public string LibraryVersion { get; set; }
}
