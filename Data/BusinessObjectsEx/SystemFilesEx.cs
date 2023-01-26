using AutoMapper;
using System.Collections.Generic;

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