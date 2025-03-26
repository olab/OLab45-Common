using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table( "map_popup_positions" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class MapPopupPositions
{
  [Key]
  [Column( "id", TypeName = "int(10) unsigned" )]
  public uint Id { get; set; }

  [Required]
  [Column( "title" )]
  [StringLength( 200 )]
  public string Title { get; set; }
}
