using DocumentFormat.OpenXml.Vml.Spreadsheet;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

public partial class Groups
{
  public const string OLabGroup = "olab";

  [NotMapped]
  public bool IsSystem
  {
    get => System == 1;
    set => System = value ? (sbyte)1 : (sbyte)0;
  }


  public override string ToString()
  {
    if ( Id != 0 )
      return $"{Name}({Id})";
    return null;
  }
}
