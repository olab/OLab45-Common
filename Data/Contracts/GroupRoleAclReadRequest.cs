using System.Collections.Generic;

namespace OLab.Data.Contracts;
public class GroupRoleAclReadRequest
{
  public const int AnySelected = 0;

  public string Types { get; set; }

  public uint? GroupId { get; set; }
  public uint? RoleId { get; set; }
  public IList<uint?> MapIds { get; set; } = new List<uint?>();
  public IList<uint?> NodeIds { get; set; } = new List<uint?>();
  public IList<uint?> AppIds { get; set; } = new List<uint?>();
}
