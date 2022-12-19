using Microsoft.AspNetCore.Http;
using OLabWebAPI.Data.Interface;
using OLabWebAPI.Data.Session;
using OLabWebAPI.Model;
using OLabWebAPI.Utils;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

#nullable disable

namespace OLabWebAPI.Data.UserContext
{
  public class UserContext : IUserContext
  {
    public const string WildCardObjectType = "*";
    public const uint WildCardObjectId = 0;

    private readonly HttpContext _httpContext;
    private readonly HttpRequest _httpRequest;
    private readonly string _accessToken;
    private IEnumerable<Claim> _claims;
    private ClaimsPrincipal _user;
    private readonly OLabDBContext _dbContext;
    private readonly OLabLogger _logger;
    protected IList<SecurityRoles> _roleAcls = new List<SecurityRoles>();
    protected IList<SecurityUsers> _userAcls = new List<SecurityUsers>();

    private IOLabSession _session;
    private string _role;
    private uint _userId;
    private string _userName;
    private string _ipAddress;

    public IOLabSession Session 
    {
        get => _session;
        set => _session = value;
    }

    public string Role 
    {
        get => _role;
        set => _role = value;
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
        get => _userName;
        set => _userName = value;
    }

    public string SessionId { get { return Session.GetSessionId(); } }

    public UserContext(OLabLogger logger, OLabDBContext dbContext)
    {
      _dbContext = dbContext;
      _logger = logger;
      Session = new OLabSession(_logger.GetLogger(), dbContext, this);
    }

    public UserContext(OLabLogger logger, OLabDBContext context, HttpRequest request)
    {
      _dbContext = context;
      _logger = logger;
      _httpRequest = request;
      _accessToken = AccessTokenUtils.ExtractBearerToken(request);

      Session = new OLabSession(_logger.GetLogger(), context, this);

      LoadHttpRequest();

    }

    public UserContext(OLabLogger logger, OLabDBContext dbContext, HttpContext httpContext)
    {
      _dbContext = dbContext;
      _logger = logger;
      _httpContext = httpContext;
      Session = new OLabSession(_logger.GetLogger(), dbContext, this);

      LoadHttpContext();
    }
    
    /// <summary>
    /// Extract claims from token
    /// </summary>
    /// <param name="token">Bearer token</param>
    private static IEnumerable<Claim> ExtractTokenClaims(string token)
    {
      JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
      JwtSecurityToken securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
      return securityToken.Claims;
    }

    protected virtual void LoadHttpRequest()
    {
      Session.SetSessionId(_httpRequest.Headers["OLabSessionId"].FirstOrDefault());
      if (!string.IsNullOrWhiteSpace(Session.GetSessionId()))
        _logger.LogInformation($"Found SessionId {Session.GetSessionId()}.");

      IPAddress = _httpRequest.Headers["X-Forwarded-Client-Ip"];
      if (string.IsNullOrEmpty(IPAddress))
        // request based requests need to get th eIPAddress using the context
        IPAddress = _httpRequest.HttpContext.Connection.RemoteIpAddress.ToString();

      _claims = ExtractTokenClaims(_accessToken);

      UserName = _claims.FirstOrDefault(c => c.Type == "name")?.Value;
      Role = _claims.FirstOrDefault(c => c.Type == "role")?.Value;

      _roleAcls = _dbContext.SecurityRoles.Where(x => x.Name.ToLower() == Role.ToLower()).ToList();

    }

    protected virtual void LoadHttpContext()
    {

      Session.SetSessionId(_httpContext.Request.Headers["OLabSessionId"].FirstOrDefault());
      if (!string.IsNullOrWhiteSpace(Session.GetSessionId()))
        _logger.LogInformation($"Found SessionId {Session.GetSessionId()}.");

      IPAddress = _httpContext.Connection.RemoteIpAddress.ToString();

      ClaimsIdentity identity = (ClaimsIdentity)_httpContext.User.Identity;
      if (identity == null)
        throw new Exception($"Unable to establish identity from token");

      _user = _httpContext.User;
      _claims = identity.Claims;

      UserName = _httpContext.Items["User"].ToString();
      Role = _httpContext.Items["Role"].ToString();
      _roleAcls = _dbContext.SecurityRoles.Where(x => x.Name.ToLower() == Role.ToLower()).ToList();

      // test for a local user
      Users user = _dbContext.Users.FirstOrDefault(x => x.Username == UserName);
      if (user != null)
      {
        UserId = user.Id;
        _userAcls = _dbContext.SecurityUsers.Where(x => x.UserId == UserId).ToList();
      }
      else
        _logger.LogWarning($"User {UserName} does not exist");

    }

    /// <summary>
    /// Test if have requested access to securable object
    /// </summary>
    /// <param name="requestedPerm">Request permissions (RWED)</param>
    /// <param name="objectType">Securable object type</param>
    /// <param name="objectId">(optional) securable object id</param>
    /// <returns>true/false</returns>
    public bool HasAccess(string requestedPerm, string objectType, uint? objectId)
    {
      int grantedCount = 0;

      for (int i = 0; i < requestedPerm.Length; i++)
      {
        if (HasSingleAccess(requestedPerm[i], objectType, objectId))
          grantedCount++;
        else
          _logger.LogError($"User {UserId}/{Role} does not have '{requestedPerm[i]}' access to '{objectType}({objectId})'");
      }

      return grantedCount == requestedPerm.Length;
    }

    /// <summary>
    /// Test if have single ACL access
    /// </summary>
    /// <param name="requestedPerm">Single-letter ACL to test for</param>
    /// <param name="objectType">Securable object type</param>
    /// <param name="objectId">(optional) securable object id</param>
    /// <returns>true/false</returns>
    private bool HasSingleAccess(char requestedPerm, string objectType, uint? objectId)
    {
      if (!objectId.HasValue)
        objectId = 0;

      bool rc = HasUserLevelAccess(requestedPerm, objectType, objectId);
      if (!rc)
        rc = HasRoleLevelAccess(requestedPerm, objectType, objectId);

      return rc;
    }

    /// <summary>
    /// Test if have single role-level ACL access
    /// </summary>
    /// <param name="requestedPerm">Single-letter ACL to test for</param>
    /// <param name="objectType">Securable object type</param>
    /// <param name="objectId">(optional) securable object id</param>
    /// <returns>true/false</returns>
    private bool HasRoleLevelAccess(char requestedPerm, string objectType, uint? objectId)
    {
      // test for most specific object acl
      SecurityRoles acl = _roleAcls.Where(x =>
       (x.ImageableType == objectType) &&
       (x.ImageableId == objectId.Value) &&
       x.Acl.Contains(requestedPerm)).FirstOrDefault();

      if (acl != null)
        return true;

      // test for specific object type acl
      acl = _roleAcls.Where(x =>
       (x.ImageableType == objectType) &&
       (x.ImageableId == WildCardObjectId) &&
       x.Acl.Contains(requestedPerm)).FirstOrDefault();

      if (acl != null)
        return true;

      // test for generic acl
      acl = _roleAcls.Where(x =>
       (x.ImageableType == WildCardObjectType) &&
       (x.ImageableId == 0) &&
       x.Acl.Contains(requestedPerm)).FirstOrDefault();

      if (acl != null)
        return true;

      return false;
    }

    /// <summary>
    /// Test if have single user-level ACL access
    /// </summary>
    /// <param name="requestedPerm">Single-letter ACL to test for</param>
    /// <param name="objectType">Securable object type</param>
    /// <param name="objectId">(optional) securable object id</param>
    /// <returns>true/false</returns>
    private bool HasUserLevelAccess(char requestedPerm, string objectType, uint? objectId)
    {

      // test for most specific object acl
      SecurityUsers acl = _userAcls.Where(x =>
       (x.ImageableType == objectType) &&
       (x.ImageableId == objectId.Value) &&
       x.Acl.Contains(requestedPerm)).FirstOrDefault();

      if (acl != null)
        return true;

      // test for specific object type acl
      acl = _userAcls.Where(x =>
       (x.ImageableType == objectType) &&
       (x.ImageableId == WildCardObjectId) &&
       x.Acl.Contains(requestedPerm)).FirstOrDefault();

      if (acl != null)
        return true;

      // test for generic acl
      acl = _userAcls.Where(x =>
       (x.ImageableType == WildCardObjectType) &&
       (x.ImageableId == 0) &&
       x.Acl.Contains(requestedPerm)).FirstOrDefault();

      if (acl != null)
        return true;

      return false;
    }
  }
}

