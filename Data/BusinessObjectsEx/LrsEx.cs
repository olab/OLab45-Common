using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Api.Model;

public partial class Lrs
{

  [NotMapped]
  public bool IsEnabled
  {
    get => Enabled == 1;
    set => Enabled = value ? (sbyte)1 : (sbyte)0;
  }

}
