using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[PrimaryKey( "ContentId", "UserId", "SubContentId", "DataId" )]
[Table( "h5p_contents_user_data" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class H5pContentsUserData
{
  [Key]
  [Column( "content_id", TypeName = "int(10) unsigned" )]
  public uint ContentId { get; set; }

  [Key]
  [Column( "user_id", TypeName = "int(10) unsigned" )]
  public uint UserId { get; set; }

  [Key]
  [Column( "sub_content_id", TypeName = "int(10) unsigned" )]
  public uint SubContentId { get; set; }

  [Key]
  [Column( "data_id" )]
  [StringLength( 127 )]
  [MySqlCharSet( "utf8mb4" )]
  [MySqlCollation( "utf8mb4_unicode_ci" )]
  public string DataId { get; set; }

  [Required]
  [Column( "data" )]
  [MySqlCharSet( "utf8mb4" )]
  [MySqlCollation( "utf8mb4_unicode_ci" )]
  public string Data { get; set; }

  [Column( "preload", TypeName = "tinyint(3) unsigned" )]
  public byte Preload { get; set; }

  [Column( "invalidate", TypeName = "tinyint(3) unsigned" )]
  public byte Invalidate { get; set; }

  [Column( "updated_at", TypeName = "timestamp" )]
  public DateTime UpdatedAt { get; set; }
}
