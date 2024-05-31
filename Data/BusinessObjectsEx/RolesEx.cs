namespace OLab.Api.Model;

public partial class Roles
{
  public const string SuperUserRole = "superuser";

  public override string ToString()
  {
    return $"{Name}({Id})";
  }
}
