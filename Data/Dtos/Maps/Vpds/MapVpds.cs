using OLab.Model;

namespace OLab.Dto
{
  public partial class MapVpdsDto
  {
    public uint Id { get; set; }
    public uint MapId { get; set; }
    public uint VpdTypeId { get; set; }
    public virtual MapVpdTypes VpdType { get; set; }
  }
}
