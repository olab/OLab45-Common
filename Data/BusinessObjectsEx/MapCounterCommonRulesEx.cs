using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Api.Model;

public partial class MapCounterCommonRules
{

  [NotMapped]
  public bool IsCorrect
  {
    get => Correct == 1;
    set => Correct = value ? (sbyte)1 : (sbyte)0;
  }

}
