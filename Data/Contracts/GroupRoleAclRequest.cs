using System.Collections.Generic;

namespace OLab.Data.Contracts;
public class GroupRoleAclRequest
{
  public const int AnySelected = 0;

  public uint? GroupId { get; set; }
  public uint? RoleId { get; set; }
  public IList<uint> MapIds { get; set; }
  public IList<uint> NodeIds { get; set; }
  public GroupRoleAclRequest()
  {
    MapIds = new List<uint>();
    NodeIds = new List<uint>();
  }
}
