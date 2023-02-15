using System;

#nullable disable

namespace OLabWebAPI.Model
{
  public partial class SecurityRoles
  {
    public override string ToString()
    {
      return $"{Id}: {Name} {ImageableType}({ImageableId}) '{Acl}'";
    }

  }
}
