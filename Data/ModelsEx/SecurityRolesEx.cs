#nullable disable

namespace OLab.Api.Models;

public partial class SecurityRoles
{
  public override string ToString()
  {
    return $"{Id}: {Name} {ImageableType}({ImageableId}) '{Acl}'";
  }

}
