namespace OLab.Api.Dto;

public class GroupRoleAclDto
{
  public bool Execute { get; set; }
  public bool Read { get; set; }
  public bool Write { get; set; }
  public string ObjectType { get; set; }
  public uint Id { get; set; }
  public uint? GroupId { get; set; }
  public uint? ObjectIndex { get; set; }
  public uint? RoleId { get; set; }
  public string GroupName { get; internal set; }
  public string RoleName { get; internal set; }
}
