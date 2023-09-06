using System.ComponentModel.DataAnnotations;

namespace OLab.Model
{
  public class ChangePasswordRequest
  {
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string NewPassword { get; set; }
  }
}