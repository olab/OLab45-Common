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

  public IList<UserGrouproles> GroupRoles
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

  string AppName
  {
    get;
    set;
  }

  string ReferringCourse
  {
    get;
    set;
  }

  //public IList<string> UserRoles { get; }

  public string ToString();
}