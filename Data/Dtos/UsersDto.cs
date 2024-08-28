using OLab.Api.Model;
using System.Collections.Generic;

namespace OLab.Data.Dtos;

public class UsersDto
{
  public uint Id { get; set; }
  public string NickName { get; set; }
  public string UserName { get; set; }
  public string Email { get; set; }

  public IList<UserGroupRolesDto> Roles { get; set; }

  public UsersDto()
  {
    Roles = new List<UserGroupRolesDto>();
  }
}
