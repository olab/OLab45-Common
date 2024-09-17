using System.Collections.Generic;

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
