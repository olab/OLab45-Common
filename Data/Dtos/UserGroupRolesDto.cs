namespace OLab.Api.Model;

public partial class UserGroupRolesDto
{
  public uint Id { get; set; }
  public string Iss { get; set; }
  public uint UserId { get; set; }
  public uint RoleId { get; set; }
  public uint GroupId { get; set; }
}
