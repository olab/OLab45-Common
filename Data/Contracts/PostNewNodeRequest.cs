using OLab.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace OLab.Data.Contracts;

public class PostNewNodeRequest
{
  [Required]
  public double X { get; set; }
  [Required]
  public double Y { get; set; }
  [Required]
  public uint SourceId { get; set; }
}

public class PostNewNodeResponse
{
  [Required]
  public MapNodeLinks Links { get; set; }
  [Required]
  public uint Id { get; set; }
}