using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table( "h5p_contents" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class H5pContents
{
  [Key]
  [Column( "id", TypeName = "int(10) unsigned" )]
  public uint Id { get; set; }

  [Column( "created_at", TypeName = "timestamp" )]
  public DateTime CreatedAt { get; set; }

  [Column( "updated_at", TypeName = "timestamp" )]
  public DateTime UpdatedAt { get; set; }

  [Column( "user_id", TypeName = "int(10) unsigned" )]
  public uint UserId { get; set; }

  [Required]
  [Column( "title" )]
  [StringLength( 255 )]
  [MySqlCharSet( "utf8mb4" )]
  [MySqlCollation( "utf8mb4_unicode_ci" )]
  public string Title { get; set; }

  [Column( "library_id", TypeName = "int(10) unsigned" )]
  public uint LibraryId { get; set; }

  [Required]
  [Column( "parameters" )]
  [MySqlCharSet( "utf8mb4" )]
  [MySqlCollation( "utf8mb4_unicode_ci" )]
  public string Parameters { get; set; }

  [Required]
  [Column( "filtered" )]
  [MySqlCharSet( "utf8mb4" )]
  [MySqlCollation( "utf8mb4_unicode_ci" )]
  public string Filtered { get; set; }

  [Required]
  [Column( "slug" )]
  [StringLength( 127 )]
  [MySqlCharSet( "utf8mb4" )]
  [MySqlCollation( "utf8mb4_unicode_ci" )]
  public string Slug { get; set; }

  [Required]
  [Column( "embed_type" )]
  [StringLength( 127 )]
  [MySqlCharSet( "utf8mb4" )]
  [MySqlCollation( "utf8mb4_unicode_ci" )]
  public string EmbedType { get; set; }

  [Column( "disable", TypeName = "int(10) unsigned" )]
  public uint Disable { get; set; }

  [Column( "content_type" )]
  [StringLength( 127 )]
  [MySqlCharSet( "utf8mb4" )]
  [MySqlCollation( "utf8mb4_unicode_ci" )]
  public string ContentType { get; set; }

  [Column( "author" )]
  [StringLength( 127 )]
  [MySqlCharSet( "utf8mb4" )]
  [MySqlCollation( "utf8mb4_unicode_ci" )]
  public string Author { get; set; }

  [Column( "license" )]
  [StringLength( 7 )]
  [MySqlCharSet( "utf8mb4" )]
  [MySqlCollation( "utf8mb4_unicode_ci" )]
  public string License { get; set; }

  [Column( "keywords", TypeName = "text" )]
  [MySqlCharSet( "utf8mb4" )]
  [MySqlCollation( "utf8mb4_unicode_ci" )]
  public string Keywords { get; set; }

  [Column( "description", TypeName = "text" )]
  [MySqlCharSet( "utf8mb4" )]
  [MySqlCollation( "utf8mb4_unicode_ci" )]
  public string Description { get; set; }
}
