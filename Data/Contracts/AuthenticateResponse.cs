using OLab.Api.Dto;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OLab.Api.Model;

public class AuthenticateResponse
{
  public DateTime CreatedAt { get; set; }
  public string UserName { get; set; }
  public IList<string> Auth { get; set; }
  public RefreshToken AuthInfo { get; set; }
  public string CourseName { get; set; }

  public AuthenticateResponse()
  {
    CreatedAt = DateTime.Now;
    AuthInfo = new RefreshToken();
    Auth = new List<string>();
  }

}