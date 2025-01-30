using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[PrimaryKey( "ContentId", "TagId" )]
[Table( "h5p_contents_tags" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class H5pContentsTags
{
  [Key]
  [Column( "content_id", TypeName = "int(10) unsigned" )]
  public uint ContentId { get; set; }

  [Key]
  [Column( "tag_id", TypeName = "int(10) unsigned" )]
  public uint TagId { get; set; }
}
