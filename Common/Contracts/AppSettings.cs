namespace OLabWebAPI.Utils
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string WebsitePublicFilesDirectory { get; set; }
        public string DefaultImportDirectory { get; set; }
        public string CertificateFile { get; set; }
        public string StaticFilesConnectionString { get; set; }
        public string StaticFilesContainerName { get; set; }
    }
    
}