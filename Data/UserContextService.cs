using Dawn;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using OLab.Api.Data.Interface;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace OLab.Api.Data;

public abstract class UserContextService : IUserContext
{
  public const string WildCardObjectType = "*";
  public const uint WildCardObjectId = 0;
  public const string NonAccessAcl = "-";
  public Model.Users OLabUser;

  private readonly IOLabLogger _logger;
  private readonly OLabDBContext _dbContext;
  protected IList<GrouproleAcls> _roleAcls = new List<GrouproleAcls>();
  protected IList<UserAcls> _userAcls = new List<UserAcls>();

  protected OLabDBContext GetDbContext() { return _dbContext; }
  protected IOLabLogger GetLogger() { return _logger; }

  private IList<UserGrouproles> _groupRoles;
  private uint _userId;
  private string _userName;
  private string _ipAddress;
  private string _issuer;
  private string _courseName;
  private string _sessionId;
  //private string _referringCourse;

  public string SessionId
  {
    get => _sessionId;
    set => _sessionId = value;
  }

  public string ReferringCourse
  {
    get => _courseName;
    set => _courseName = value;
  }

  public IList<UserGrouproles> GroupRoles
  {
    get => _groupRoles;
    set => _groupRoles = value;
  }

  public uint UserId
  {
    get => _userId;
    set => _userId = value;
  }

  public string UserName
  {
    get => _userName;
    set => _userName = value;
  }

  public string IPAddress
  {
    get => _ipAddress;
    set => _ipAddress = value;
  }

  public string Issuer
  {
    get => _issuer;
    set => _issuer = value;
  }
  string IUserContext.SessionId
  {
    get => _sessionId;
    set => _sessionId = value;
  }

  public string CourseName { get { return _courseName; } }

  protected IDictionary<string, string> Claims;
  protected IDictionary<string, string> Headers;

  // default ctor, needed for services Dependancy Injection
  public UserContextService()
  {
  }

  public UserContextService(
    IOLabLogger logger,
    OLabDBContext dbContext)
  {
    Guard.Argument(logger).NotNull(nameof(logger));
    Guard.Argument(dbContext).NotNull(nameof(dbContext));

    _logger = logger;
    _dbContext = dbContext;
  }

  protected string GetClaimValue(string key, bool isRequired = true)
  {
    if (Claims.TryGetValue(key, out var value))
      return value;

    if (isRequired)
      throw new Exception($"claim value '{key}' does not exist");

    return string.Empty;
  }

  protected string GetHeaderValue(string key, bool isRequired = true)
  {
    if (Headers.TryGetValue(key, out var value))
      return value;

    if ( isRequired )
      throw new Exception($"header value '{key}' does not exist");

    return string.Empty;
  }

  protected void LoadContext()
  {
    var sessionId = GetHeaderValue("OLabSessionId".ToLower(), false);
    if (sessionId != string.Empty)
    {
      if (!string.IsNullOrEmpty(sessionId) && sessionId != "null")
      {
        SessionId = sessionId;
        if (!string.IsNullOrWhiteSpace(SessionId))
          GetLogger().LogInformation($"Found sessionId '{SessionId}'.");
      }
    }

    UserName = GetClaimValue(ClaimTypes.Name);
    ReferringCourse = GetClaimValue(ClaimTypes.UserData);
    Issuer = GetClaimValue("iss");
    UserId = (uint)Convert.ToInt32(GetClaimValue("id"));

    var groupRoleString = GetClaimValue(ClaimTypes.Role);
    GroupRoles = UserGrouproles.StringToObjectList(GetDbContext(), groupRoleString);
  }

  public override string ToString()
  {
    return $"{UserId} {Issuer} {UserName} {UserGrouproles.ListToString(GroupRoles)} {IPAddress} {ReferringCourse}";
  }
}
