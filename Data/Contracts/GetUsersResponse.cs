using OLab.Data.Dtos;
using System.Collections.Generic;

namespace OLab.Data.Contracts;
public class GetUsersResponse
{
  public IList<UsersDto> Users { get; set; }
}
