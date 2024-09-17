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
}
