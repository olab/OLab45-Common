namespace OLab.Api.Model;

public partial class Groups
{
  public const string OLabGroup = "olab";

  public override string ToString()
  {
    if ( Id != 0 )
      return $"{Name}({Id})";
    return null;
  }
}
