using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OLab.Api.Model
{
  public class AssignSecurityUserRequest
  {
    [Required]
    public uint UserId { get; set; }
    [Required]
    public string Acl { get; set; }
    public readonly string[] AllowedAcls = { "R", "W", "X", "D" };

    public void CheckAcl()
    {
      foreach (var part in Acl)
      {
        if (!AllowedAcls.Any(val => part.ToString() == val))
          throw new System.Exception("Bad ACL value");
      }
    }
  }
}