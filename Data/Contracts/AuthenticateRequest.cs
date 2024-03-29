using System.ComponentModel.DataAnnotations;

namespace OLab.Api.Model;

public class AuthenticateRequest
{
  [Required]
  public string Username { get; set; }

  [Required]
  public string Password { get; set; }

  public string NewPassword { get; set; }
}