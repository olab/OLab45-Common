using NuGet.Packaging;
using OLab.Api.Model;
using System.Collections.Generic;

namespace OLab.Data.Dtos;

public class UsersImportDto : UsersDto
{
  public bool Status { set; get; } = true;
  public string Message { get; set; }

  public UsersImportDto()
  {
  }

  public UsersImportDto(UsersDto user)
  {
    this.Id = user.Id;
    this.NickName = user.NickName;
    this.UserName = user.UserName;
    this.Email = user.Email;
    this.Roles.AddRange(user.Roles);
  }
}
