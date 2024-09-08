namespace OLab.Api.Model;

public partial class Groups
{
  public const string OLabGroup = "olab";

  public override string ToString()
  {
    return $"{Name}({Id})";
  }
}
