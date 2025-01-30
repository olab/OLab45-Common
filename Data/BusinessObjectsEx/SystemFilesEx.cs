#nullable disable

namespace OLab.Api.Model;

public partial class SystemFiles
{
  /// <summary>
  /// Host name override, if store is in 
  /// a different location
  /// </summary>
  [System.ComponentModel.DataAnnotations.Schema.NotMapped]
  public string HostName { get; set; }

  public override string ToString()
  {
    return $"{Name}({Id}): {Path}";
  }
}