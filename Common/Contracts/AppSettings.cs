namespace OLab.Api.Utils
{
  public class AppSettings
  {
    public string Audience { get; set; }
    public string FileStorageConnectionString { get; set; }
    public string FileStorageContainer { get; set; }
    public string ImportFolder { get; set; }
    public string Issuer { get; set; }
    public string PublicFileFolder { get; set; }
    public string Secret { get; set; }
    public string SignalREndpoint { get; set; }
  }
}