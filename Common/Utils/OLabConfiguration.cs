using Dawn;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OLab.Api.Utils;
using OLab.Common.Interfaces;

namespace OLab.Common.Utils;

public class OLabConfiguration : IOLabConfiguration
{
  public const string AppSettingPrefix = "AppSettings";

  private readonly IConfiguration _configuration;
  private readonly AppSettings _appSettings;

  public OLabConfiguration(
    ILoggerFactory loggerFactory,
    IConfiguration configuration)
  {
    Guard.Argument( loggerFactory ).NotNull( nameof( loggerFactory ) );
    Guard.Argument( configuration ).NotNull( nameof( configuration ) );

    _configuration = configuration;
    var logger = OLabLogger.CreateNew<OLabConfiguration>( loggerFactory );

    //logger.LogInformation( $"Configuration:" );

    //foreach ( var item in _configuration.AsEnumerable() )
    //  logger.LogInformation( $"  {item.Key} -> {item.Value}" );

    _appSettings = new AppSettings
    {
      Audience = _configuration.GetValue<string>( "Audience" ),
      Issuer = _configuration.GetValue<string>( "Issuer" ),
      Secret = _configuration.GetValue<string>( "Secret" ),
      TokenExpiryMinutes = _configuration.GetValue<int>( "TokenExpiryMinutes" ),
      FileStorageRoot = _configuration.GetValue<string>( "FileStorageRoot" ),
      FileStorageUrl = _configuration.GetValue<string>( "FileStorageUrl" ),
      FileStorageType = _configuration.GetValue<string>( "FileStorageType" ),
      FileStorageConnectionString = _configuration.GetValue<string>( "FileStorageConnectionString" )
    };

    var json = JsonConvert.SerializeObject( _appSettings );
    logger.LogInformation( json );

  }

  public IConfiguration GetRawConfiguration() { return _configuration; }

  //public IConfiguration GetConfiguration() { return _configuration; }
  public AppSettings GetAppSettings() { return _appSettings; }

}
