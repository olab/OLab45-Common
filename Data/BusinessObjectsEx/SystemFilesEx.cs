#nullable disable

namespace OLab.Model
{
  public partial class SystemFiles
  {
    public override string ToString()
    {
      return $"{Name}({Id}): {Path}";
    }
  }
}