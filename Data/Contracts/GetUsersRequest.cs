using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLab.Data.Contracts;

public class GetUsersRequest
{
  public IList<string> GroupsRoles { get; set; }
  public string UserSearchTerm { get; set; }

  public GetUsersRequest()
  {
    GroupsRoles = new List<string>();
  }
}
