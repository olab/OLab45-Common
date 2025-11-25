#nullable disable

using System.ComponentModel.DataAnnotations.Schema;

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

  [NotMapped]
  public bool IsRaw
  {
    get => Raw == 1;
    set => Raw = value ? (sbyte)1 : (sbyte)0;
  }

  public override string ToString()
  {
    return $"{Name}({Id}): {OriginUrl}";
  }
}