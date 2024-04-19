using OLab.Api.Model;
using System.Collections.Generic;

namespace OLab.Api.Data.Interface;

public interface IUserContext
{
  public string SessionId
  {
    get;
    set;
  }

  public string Role
  {
    get;
    set;
  }

  public uint UserId
  {
    get;
    set;
  }

  public string UserName
  {
    get;
    set;
  }

  public string IPAddress
  {
    get;
    set;
  }
  public string Issuer
  {
    get;
    set;
  }

  string ReferringCourse { get; }

  public IList<UserGroups> UserRoles { get; }

  public bool IsMemberOf(string groupName, string RoleName);

  public string ToString();
}