using System.ComponentModel.DataAnnotations;

namespace OLab.Model
{
  public class ExternalLoginRequest
  {
    [Required]
    public string ExternalToken { get; set; }
  }
}