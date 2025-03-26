using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table( "author_rights" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class AuthorRights
{
  [Key]
  [Column( "id", TypeName = "int(10) unsigned" )]
  public uint Id { get; set; }

  [Column( "map_id", TypeName = "int(10) unsigned" )]
  public uint MapId { get; set; }

  [Column( "user_id", TypeName = "int(10) unsigned" )]
  public uint UserId { get; set; }
}
