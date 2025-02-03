using Dawn;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using System;

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

    _appSettings = new AppSettings();

    // handle case where settings are in 'AppSettings' section or
    // as environment variables
    var sect = configuration.GetSection( "AppSettings" );
    if ( sect.Exists() )
      configuration.GetSection( "AppSettings" ).Bind( _appSettings );
    else
    {

      var properties = typeof( AppSettings ).GetProperties();
      foreach ( var property in properties )
      {
        var envValue = Environment.GetEnvironmentVariable( property.Name );
        if ( !string.IsNullOrEmpty( envValue ) )
        {
          var convertedValue = Convert.ChangeType( envValue, property.PropertyType );
          property.SetValue( _appSettings, convertedValue );
        }
      }
    }

    var json = JsonConvert.SerializeObject( _appSettings, Formatting.Indented );
    Console.WriteLine( $" Configuration {json}" );

  }

  public IConfiguration GetRawConfiguration() { return _configuration; }

  //public IConfiguration GetConfiguration() { return _configuration; }
  public AppSettings GetAppSettings() { return _appSettings; }

}
