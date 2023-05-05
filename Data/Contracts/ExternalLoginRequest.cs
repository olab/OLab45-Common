using System.ComponentModel.DataAnnotations;

namespace OLabWebAPI.Model
{
  public class ExternalLoginRequest
  {
    [Required]
    public string ExternalToken { get; set; }
  }
}