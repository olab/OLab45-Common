using Dawn;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
    Guard.Argument(loggerFactory).NotNull(nameof(loggerFactory));
    Guard.Argument(configuration).NotNull(nameof(configuration));

    _configuration = configuration;
    _appSettings = _configuration.GetSection("AppSettings").Get<AppSettings>();

    var logger = OLabLogger.CreateNew<OLabConfiguration>(loggerFactory);

    foreach (var item in configuration.AsEnumerable())
      logger.LogDebug($"{item.Key} -> {item.Value}");

  }

  public IConfiguration GetRawConfiguration() { return _configuration; }

  //public IConfiguration GetConfiguration() { return _configuration; }
  public AppSettings GetAppSettings() { return _appSettings; }

}
