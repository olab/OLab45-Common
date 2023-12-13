using System.ComponentModel.DataAnnotations;

namespace OLab.Data.Contracts
{
  public class ExternalLoginRequest
  {
    [Required]
    public string ExternalToken { get; set; }
  }
}