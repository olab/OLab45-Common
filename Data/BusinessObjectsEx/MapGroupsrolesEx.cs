namespace OLab.Api.Model;

public partial class MapGrouproles
{
  public MapGrouproles()
  {

  }
  public MapGrouproles(uint mapId, uint? groupId, uint? roleId)
  {
    MapId = mapId;
    GroupId = groupId;
    RoleId = roleId;
  }

  public override string ToString()
  {
    var group = Group == null ? GroupId.ToString() : Group.Name;
    var role = Role == null ? RoleId.ToString() : Role.Name;
    return $"{group}:{role}";
  }
}
