using System.ComponentModel.DataAnnotations;

namespace OLab.Data.Contracts;

public class PutNodeGridRequest
{
  [Required]
  public uint Id { get; set; }
  [Required]
  public string Text { get; set; }
  [Required]
  public string Title { get; set; }
  [Required]
  public double X { get; set; }
  [Required]
  public double Y { get; set; }
}