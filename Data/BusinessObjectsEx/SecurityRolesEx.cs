#nullable disable

namespace OLab.Api.Model;

public partial class SecurityRoles
{
  public override string ToString()
  {
    return $"{Id}: {Role} {ImageableType}({ImageableId}) '{Acl}'";
  }

}
