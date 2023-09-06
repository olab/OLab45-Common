namespace OLab.Api.Data.Interface
{
  public interface IUserContext
  {
    public IOLabSession Session
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

    bool HasAccess(string requestedPerm, string objectType, uint? objectId);

  }
}