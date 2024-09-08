using Newtonsoft.Json;

namespace OLab.Api.Dto;

public partial class MapNodeGroupRolesDto
{
  [JsonProperty("id")]
  public uint? Id { get; set; }
  [JsonProperty("nodeId")]
  public uint NodeId { get; set; }
  [JsonProperty("groupId")]
  public uint GroupId { get; set; }
  [JsonProperty("groupName")]
  public string GroupName { get; set; }
  [JsonProperty("roleId")]
  public uint RoleId { get; set; }
  [JsonProperty("roleName")]
  public string RoleName { get; set; }
}
