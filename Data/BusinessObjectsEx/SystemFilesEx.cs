#nullable disable

namespace OLabWebAPI.Model
{
  public partial class SystemFiles
  {
    public override string ToString()
    {
      return $"{Name}({Id}): {Path}";
    }
  }
}