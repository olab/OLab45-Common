using OLab.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLab.Data.Dtos;

public class UsersDto
{
  public uint Id { get; set; }
  public string NickName { get; set; }
  public string UserName { get; set; }
  public IList<UserGroupRolesDto> Roles { get; set; }

  public UsersDto()
  {
    Roles = new List<UserGroupRolesDto>();
  }
}
