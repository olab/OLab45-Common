using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Api.Model;

public partial class MapNodeJumps
{

  [NotMapped]
  public bool IsHidden
  {
    get => Hidden == 1;
    set => Hidden = value ? (sbyte)1 : (sbyte)0;
  }

}
