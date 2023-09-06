using System.ComponentModel.DataAnnotations;

namespace OLab.Api.Model
{
  public class ExternalLoginRequest
  {
    [Required]
    public string ExternalToken { get; set; }
  }
}