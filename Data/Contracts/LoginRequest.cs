using System.ComponentModel.DataAnnotations;

namespace OLab.Data.Contracts
{
  public class LoginRequest
  {
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
  }
}