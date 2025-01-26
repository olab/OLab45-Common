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
  private IDictionary<string, string> _headers = new Dictionary<string, string>();

  public IDictionary<string, string> Claims 
  { 
    get { return _claims; }  
  }

  public IDictionary<string, string> Headers 
  { 
    get { return _headers; } 
  }

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

  protected void SetHeaders(IDictionary<string, string> headers)
  {
    foreach ( var header in headers )
      _headers.Add( header.Key.ToLower(), header.Value );

    GetLogger().LogInformation( $"found {Headers.Count} headers" );

    //foreach ( var header in _headers )
    //  _logger.LogInformation( $"  header: {header.Key} = {header.Value}" );
  }

  protected void SetClaims(IDictionary<string, string> claims)
  {
    foreach ( var claim in claims )
      _claims.Add( claim.Key.ToLower(), claim.Value );

    GetLogger().LogInformation( $"found {Claims.Count} claims" );

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

  protected void LoadUserContext()
  {
    if ( _headers.Count == 0 )
      throw new Exception( "no headers found" );

    var sessionId = GetHeader( "sessionid", false );
    if ( sessionId != string.Empty )
    {
      if ( !string.IsNullOrEmpty( sessionId ) && sessionId != "null" )
      {
        SessionId = sessionId;
        if ( !string.IsNullOrWhiteSpace( SessionId ) )
          GetLogger().LogInformation( $"Found sessionId '{SessionId}'." );
      }
    }
    else
      _logger.LogWarning( "no OLabSessionId provided" );

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

  /// <summary>
  /// Retrieves the value of a specified header from the request headers.
  /// </summary>
  /// <param name="key">The key of the header to retrieve.</param>
  /// <param name="isRequired">Indicates whether the header is required. If true, an exception is thrown if the header is not found.</param>
  /// <returns>The value of the specified header if found; otherwise, an empty string if the header is not required and not found.</returns>
  /// <exception cref="Exception">Thrown if the header is required and not found.</exception>
  protected string GetHeader(string key, bool isRequired = true)
  {
    if ( _headers.TryGetValue( key.ToLower(), out var value ) )
      return value;

    if ( isRequired )
      throw new Exception( $"header value '{key}' does not exist" );

    return string.Empty;
  }

  public override string ToString()
  {
    return $"{UserId} {Issuer} {UserName} {UserGrouproles.ListToString( GroupRoles )} {IPAddress} {ReferringCourse}";
  }
}
