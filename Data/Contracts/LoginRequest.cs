using System.ComponentModel.DataAnnotations;

namespace OLab.Api.Model;

public class LoginRequest
{
  [Required]
  public string Username { get; set; }
  public string Password { get; set; }
}