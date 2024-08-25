using System.Collections.Generic;

namespace OLab.Data.Contracts;
public class GroupRoleAclRequest
{
  public int? GroupId { get; set; }
  public int? RoleId { get; set; }
  public IList<uint> MapIds { get; set; }
  public IList<uint> NodeIds { get; set; }
  public GroupRoleAclRequest()
  {
    MapIds = new List<uint>();
    NodeIds = new List<uint>();
  }
}
