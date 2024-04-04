using OLab.Api.Dto;
using System.Collections.Generic;

namespace OLab.Api.Model;

public class AddUserResponse
{
  public uint Id { get; set; }
  public string Username { get; set; }
  public string Password { get; set; }
  public string Message { get; set; }
  public string Auth {  get; set; }

  public AddUserResponse()
  {
    Message = "OK";
  }
}