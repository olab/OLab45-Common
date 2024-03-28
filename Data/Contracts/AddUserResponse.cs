using System.Collections;
using System.Collections.Generic;

namespace OLab.Api.Model;

public class AddUserResponse
{
  public uint Id { get; set; }
  public string Username { get; set; }
  public string Password { get; set; }
  public string Message { get; set; }
  public IList<GroupRole> Roles { get; set; }

  public AddUserResponse()
  {
    Roles = new List<GroupRole>();
    Message = "OK";
  }
}