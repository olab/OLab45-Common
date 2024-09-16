namespace OLab.Api.Model;

public partial class Roles
{
  public const string SuperUserRole = "superuser";
  public const string AuthorRole = "author";
  public const string DirectorRole = "director";

  public override string ToString()
  {
    if (Id != 0)
      return $"{Name}({Id})";
    return null;
  }
}
