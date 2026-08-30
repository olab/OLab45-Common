using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Api.Model;

public partial class UserSessiontraces
{

  [NotMapped]
  public bool IsRedirected
  {
    get => Redirected == 1;
    set => Redirected = value ? (sbyte)1 : (sbyte)0;
  }

}
