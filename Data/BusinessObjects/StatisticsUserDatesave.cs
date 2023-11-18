using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Api.Model
{
  [Table("statistics_user_datesave")]
  public partial class StatisticsUserDatesave
  {
    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }
    [Column("date_save", TypeName = "int(10)")]
    public int DateSave { get; set; }
  }
}
