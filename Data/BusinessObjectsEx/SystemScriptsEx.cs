#nullable disable

namespace OLab.Api.Model;

public partial class SystemScripts
{
  /// <summary>
  /// Host name override, if store is in 
  /// a different location
  /// </summary>
  [System.ComponentModel.DataAnnotations.Schema.NotMapped]
  public string HostName { get; set; }

  [System.ComponentModel.DataAnnotations.Schema.NotMapped]
  public string OriginUrl { get; set; }

  public override string ToString()
  {
    return $"{Name}({Id}): {OriginUrl}";
  }
}