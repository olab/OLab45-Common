using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Api.Model;

public partial class UserSessions
{

  [NotMapped]
  public bool IsNotCumulative
  {
    get => NotCumulative == 1;
    set => NotCumulative = value ? (sbyte)1 : (sbyte)0;
  }

}
