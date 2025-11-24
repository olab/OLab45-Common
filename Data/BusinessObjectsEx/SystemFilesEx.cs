#nullable disable

using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

public partial class SystemFiles
{
  [NotMapped]
  public bool IsShared
  {
    get => Shared == 1;
    set => Shared = value ? (sbyte)1 : (sbyte)0;
  }

  [NotMapped]
  public bool IsPrivate
  {
    get => Private == 1;
    set => Private = value ? (sbyte)1 : (sbyte)0;
  }

  [NotMapped]
  public bool IsEmbedded
  {
    get => Embedded == 1;
    set => Embedded = value ? (sbyte)1 : (sbyte)0;
  }

  [NotMapped]
  public bool? IsMedia
  {
    get => Media.HasValue ? Media.Value == 1 : (bool?)null;
    set => Media = value.HasValue ? (value.Value ? (sbyte)1 : (sbyte)0) : (sbyte?)null;
  }

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