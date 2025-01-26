using Dawn;
using DocumentFormat.OpenXml.Spreadsheet;
using OLab.Api.Data.Interface;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace OLab.Api.Data;

public abstract class UserContext : IUserContext
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

  private IList<UserGrouproles> _groupRoles = new List<UserGrouproles>();
  private uint _userId;
  private string _userName;
  private string _ipAddress;
  private string _issuer;
  private string _courseName;
  private string _sessionId;
  private string _appName;

  public string CourseName { get { return _courseName; } }

  private IDictionary<string, string> _claims = new Dictionary<string, string>();

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
  public string AppName
  {
    get => _appName;
    set => _appName = value;
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

  // default ctor, needed for services Dependancy Injection
  public UserContext()
  {
  }

  public UserContext(
    IOLabLogger logger,
    OLabDBContext dbContext)
  {
    Guard.Argument( logger ).NotNull( nameof( logger ) );
    Guard.Argument( dbContext ).NotNull( nameof( dbContext ) );

    _logger = logger;
    _dbContext = dbContext;
  }

  protected void SetClaims(IDictionary<string, string> claims)
  {
    foreach ( var claim in claims )
      _claims.Add( claim.Key.ToLower(), claim.Value );

    //foreach ( var claim in _claims )
    //  _logger.LogInformation( $"  claim: {claim.Key} = {claim.Value}" );
  }

  protected string GetClaim(string key, bool isRequired = true)
  {
    if ( _claims.TryGetValue( key.ToLower(), out var value ) )
      return value;

    if ( isRequired )
      throw new Exception( $"claim value '{key}' does not exist" );

    return string.Empty;
  }

  protected void LoadContext()
  {
    var sessionId = GetHeader( "OLabSessionId", false );
    if ( sessionId != string.Empty )
    {
      if ( !string.IsNullOrEmpty( sessionId ) && sessionId != "null" )
      {
        SessionId = sessionId;
        if ( !string.IsNullOrWhiteSpace( SessionId ) )
          GetLogger().LogInformation( $"Found sessionId '{SessionId}'." );
      }
    }

    // add special case to detect 2 possible forms of the 'name' claim
    // "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" or "name"
    UserName = GetClaim( ClaimTypes.Name, false );
    if ( string.IsNullOrEmpty( UserName ) )
      UserName = GetClaim( "name" );

    ReferringCourse = "olabinternal";
    ReferringCourse = GetClaim( ClaimTypes.UserData, false );
    Issuer = GetClaim( "iss" );
    UserId = (uint)Convert.ToInt32( GetClaim( "id" ) );
    AppName = GetClaim( "app" );

    // add special case to detect 2 possible forms of the 'role' claim
    // "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role" or "role"
    var groupRoleString = GetClaim( ClaimTypes.Role, false );
    if ( string.IsNullOrEmpty( groupRoleString ) )
      groupRoleString = GetClaim( "role" );

    GroupRoles = UserGrouproles.StringToObjectList( GetDbContext(), groupRoleString );
  }

  public override string ToString()
  {
    return $"{UserId} {Issuer} {UserName} {UserGrouproles.ListToString( GroupRoles )} {IPAddress} {ReferringCourse}";
  }
}
