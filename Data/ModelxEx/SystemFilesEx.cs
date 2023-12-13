#nullable disable

namespace OLab.Api.Model
{
  public partial class SystemFiles
  {
    public override string ToString()
    {
      return $"{Name}({Id}): {Path}";
    }
  }
}