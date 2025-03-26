using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table( "map_popups_assign" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class MapPopupsAssign
{
  [Key]
  [Column( "id", TypeName = "int(10) unsigned" )]
  public uint Id { get; set; }

  [Column( "map_popup_id", TypeName = "int(10) unsigned" )]
  public uint MapPopupId { get; set; }

  [Column( "assign_type_id", TypeName = "int(10) unsigned" )]
  public uint AssignTypeId { get; set; }

  [Column( "assign_to_id", TypeName = "int(10) unsigned" )]
  public uint AssignToId { get; set; }

  [Column( "redirect_to_id", TypeName = "int(10) unsigned" )]
  public uint? RedirectToId { get; set; }

  [Column( "redirect_type_id", TypeName = "int(10) unsigned" )]
  public uint RedirectTypeId { get; set; }
}
