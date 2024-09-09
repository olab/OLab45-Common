using OLab.Data.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLab.Data.Contracts;
public class GetUsersResponse
{
  public IList<UsersDto> Users { get; set; }
}
