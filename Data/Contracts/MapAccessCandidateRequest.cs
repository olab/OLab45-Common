using System.ComponentModel.DataAnnotations;

namespace OLab.Api.Model
{
  public class MapAccessCandidateRequest
  {
    [Required]
    public uint MapId { get; set; }
    [Required]
    public string Username { get; set; }
    [Required]
    public string Email { get; set; }
  }
}