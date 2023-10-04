namespace OLab.Api.Utils
{
  public class AppSettings
  {
    public string Audience { get; set; }
    public string FileStorageConnectionString { get; set; }
    public string FileStorageContainer { get; set; }
    public string FileStorageFolder { get; set; }
    public string FileImportFolder{ get; set; }
    public string FileStorageType { get; set; }
    public string FileStorageUrl { get; set; }
    public string ImportFolder { get; set; }
    public string Issuer { get; set; }
    public string Secret { get; set; }
    public string SignalREndpoint { get; set; }
    public int TokenExpiryMinutes { get; set; }

  }
}