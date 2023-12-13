#nullable disable

namespace OLab.Api.Models
{
  public partial class SystemFiles
  {
    public override string ToString()
    {
      return $"{Name}({Id}): {Path}";
    }
  }
}