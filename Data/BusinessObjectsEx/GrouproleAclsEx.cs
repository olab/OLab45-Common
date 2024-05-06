#nullable disable

namespace OLab.Api.Model;

public partial class GrouproleAcls
{
  public override string ToString()
  {
    return $"{Id}: {Name} {ImageableType}({ImageableId}) '{Acl}'";
  }

}
