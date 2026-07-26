using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table( "statistics_user_datesave" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class StatisticsUserDatesave
{
  [Key]
  [Column( "id" )]
  public uint Id { get; set; }

  [Column( "date_save" )]
  public int DateSave { get; set; }
}
