using System.ComponentModel.DataAnnotations;

namespace OLab.Model
{
  public class PostNewLinkRequest
  {
    [Required]
    public uint DestinationId { get; set; }
  }

  public class PostNewLinkResponse
  {
    [Required]
    public uint Id { get; set; }
  }
}