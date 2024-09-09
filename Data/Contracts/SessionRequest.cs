using System;

namespace OLab.Data.Contracts;
public class SessionRequest
{
  public uint? MapId { get; set; }
  public DateTime? CourseDate { get; set; }
  public uint? UserId { get; set; }
  public string Issuer { get; set; }
  public string ContextId { get; set; }
}
