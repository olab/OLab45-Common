using Dawn;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Newtonsoft.Json.Linq;
using OLab.Api.Data.Interface;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;
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

  private IDictionary<string, string> _claims = new Dictionary<string, string>();
  private IDictionary<string, string> _headers = new Dictionary<string, string>();

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

  protected void SetClaims(IDictionary<string, string> headers)
  {
    _headers = headers;
    GetLogger().LogInformation($"Headers:");
    foreach (var item in _headers)
      GetLogger().LogInformation($" '{item.Key}'");
  }

  protected void SetHeaders(IDictionary<string, string> claims)
  {
    _claims = claims;
    GetLogger().LogInformation($"Claims:");
    foreach (var item in _claims)
      GetLogger().LogInformation($" '{item.Key}'");
  }

  protected string GetClaimValue(string key, bool isRequired = true)
  {
    if (_claims.TryGetValue(key, out var value))
      return value;

    if (isRequired)
      throw new Exception($"claim value '{key}' does not exist");

    return string.Empty;
  }

  protected string GetHeaderValue(string key, bool isRequired = true)
  {
    if (_headers.TryGetValue(key, out var value))
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
    return $"{UserId} {Issuer} {UserName} {GroupRoles} {IPAddress} {ReferringCourse}";
  }
}
