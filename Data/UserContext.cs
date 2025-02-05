using Dawn;
using OLab.Api.Data.Interface;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace OLab.Api.Data;

/// <summary>
/// Represents the user context within the OLab application.
/// </summary>
public abstract class UserContext : IUserContext
{
  public const string HEADER_SESSIONID = "olabsessionid";
  public const string WildCardObjectType = "*";
  public const uint WildCardObjectId = 0;
  public const string NonAccessAcl = "-";
  public Model.Users OLabUser;

  private readonly IOLabLogger _logger;
  private readonly OLabDBContext _dbContext;
  protected IList<GrouproleAcls> _roleAcls = new List<GrouproleAcls>();
  protected IList<UserAcls> _userAcls = new List<UserAcls>();

  /// <summary>
  /// Gets the database context.
  /// </summary>
  /// <returns>The database context.</returns>
  protected OLabDBContext GetDbContext() { return _dbContext; }

  /// <summary>
  /// Gets the logger.
  /// </summary>
  /// <returns>The logger.</returns>
  protected IOLabLogger GetLogger() { return _logger; }

  private IList<UserGrouproles> _groupRoles = new List<UserGrouproles>();
  private uint _userId;
  private string _userName;
  private string _ipAddress;
  private string _issuer;
  private string _courseName;
  private string _sessionId;
  private string _appName;

  /// <summary>
  /// Gets the course name.
  /// </summary>
  public string CourseName { get { return _courseName; } }

  private readonly IDictionary<string, string> _claims = new Dictionary<string, string>();
  private readonly IDictionary<string, string> _headers = new Dictionary<string, string>();

  /// <summary>
  /// Gets the claims.
  /// </summary>
  public IDictionary<string, string> Claims
  {
    get { return _claims; }
  }

  /// <summary>
  /// Gets the headers.
  /// </summary>
  public IDictionary<string, string> Headers
  {
    get { return _headers; }
  }

  /// <summary>
  /// Gets or sets the session ID.
  /// </summary>
  public string SessionId
  {
    get => _sessionId;
    set => _sessionId = value;
  }

  /// <summary>
  /// Gets or sets the referring course.
  /// </summary>
  public string ReferringCourse
  {
    get => _courseName;
    set => _courseName = value;
  }

  /// <summary>
  /// Gets or sets the group roles.
  /// </summary>
  public IList<UserGrouproles> GroupRoles
  {
    get => _groupRoles;
    set => _groupRoles = value;
  }

  /// <summary>
  /// Gets or sets the user ID.
  /// </summary>
  public uint UserId
  {
    get => _userId;
    set => _userId = value;
  }

  /// <summary>
  /// Gets or sets the application name.
  /// </summary>
  public string AppName
  {
    get => _appName;
    set => _appName = value;
  }

  /// <summary>
  /// Gets or sets the user name.
  /// </summary>
  public string UserName
  {
    get => _userName;
    set => _userName = value;
  }

  /// <summary>
  /// Gets or sets the IP address.
  /// </summary>
  public string IPAddress
  {
    get => _ipAddress;
    set => _ipAddress = value;
  }

  /// <summary>
  /// Gets or sets the issuer.
  /// </summary>
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

  /// <summary>
  /// Initializes a new instance of the <see cref="UserContext"/> class.
  /// </summary>
  public UserContext()
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="UserContext"/> class with the specified logger and database context.
  /// </summary>
  /// <param name="logger">The logger.</param>
  /// <param name="dbContext">The database context.</param>
  public UserContext(
    IOLabLogger logger,
    OLabDBContext dbContext)
  {
    Guard.Argument( logger ).NotNull( nameof( logger ) );
    Guard.Argument( dbContext ).NotNull( nameof( dbContext ) );

    _logger = logger;
    _dbContext = dbContext;

    GetLogger().LogInformation( $"UserContext ctor" );
  }

  /// <summary>
  /// Sets the headers.
  /// </summary>
  /// <param name="headers">The headers to set.</param>
  protected void SetHeaders(IDictionary<string, string> headers)
  {
    foreach ( var header in headers )
      _headers.Add( header.Key.ToLower(), header.Value );

    GetLogger().LogInformation( $"found {Headers.Count} headers" );
  }

  /// <summary>
  /// Sets the claims.
  /// </summary>
  /// <param name="claims">The claims to set.</param>
  protected void SetClaims(IDictionary<string, string> claims)
  {
    foreach ( var claim in claims )
      _claims.Add( claim.Key.ToLower(), claim.Value );

    GetLogger().LogInformation( $"found {Claims.Count} claims" );
  }

  /// <summary>
  /// Gets the claim value for the specified key.
  /// </summary>
  /// <param name="key">The key of the claim.</param>
  /// <param name="isRequired">Indicates whether the claim is required.</param>
  /// <returns>The claim value.</returns>
  /// <exception cref="Exception">Thrown if the claim is required and not found.</exception>
  protected string GetClaim(string key, bool isRequired = true)
  {
    if ( _claims.TryGetValue( key.ToLower(), out var value ) )
      return value;

    if ( isRequired )
      throw new Exception( $"claim value '{key}' does not exist" );

    return string.Empty;
  }

  /// <summary>
  /// Loads the user context.
  /// </summary>
  /// <exception cref="Exception">Thrown if no headers are found.</exception>
  protected void LoadUserContext()
  {
    if ( _headers.Count == 0 )
      throw new Exception( "no headers found" );

    var sessionId = GetHeader( HEADER_SESSIONID, false );
    if ( sessionId != string.Empty )
    {
      if ( !string.IsNullOrEmpty( sessionId ) && sessionId != "null" )
      {
        SessionId = sessionId;
        if ( !string.IsNullOrWhiteSpace( SessionId ) )
          GetLogger().LogInformation( $"Found {HEADER_SESSIONID} '{SessionId}'." );
      }
    }
    else
      _logger.LogWarning( $"no {HEADER_SESSIONID} provided" );

    UserName = GetClaim( ClaimTypes.Name, false );
    if ( string.IsNullOrEmpty( UserName ) )
      UserName = GetClaim( "name" );

    ReferringCourse = "olabinternal";
    ReferringCourse = GetClaim( ClaimTypes.UserData, false );
    Issuer = GetClaim( "iss" );
    UserId = (uint)Convert.ToInt32( GetClaim( "id" ) );
    AppName = GetClaim( "app" );

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

  /// <summary>
  /// Returns a string that represents the current object.
  /// </summary>
  /// <returns>A string that represents the current object.</returns>
  public override string ToString()
  {
    return $"{UserId} {Issuer} {UserName} {UserGrouproles.ListToString( GroupRoles )} {IPAddress} {ReferringCourse}";
  }
}
