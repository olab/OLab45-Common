#nullable disable

namespace OLab.Model
{
  public partial class SecurityRoles
  {
    public override string ToString()
    {
      return $"{Id}: {Name} {ImageableType}({ImageableId}) '{Acl}'";
    }

  }
}
