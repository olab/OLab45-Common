using Dawn;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using System;
using System.Reflection;

namespace OLab.Common.Utils;

public class OLabConfiguration : IOLabConfiguration
{
  public const string AppSettingPrefix = "AppSettings";

  private readonly IConfiguration _configuration;
  private readonly IOptions<AppSettings> _appSettings;

  public OLabConfiguration(
    ILoggerFactory loggerFactory,
    IConfiguration configuration)
  {
    Guard.Argument(loggerFactory).NotNull(nameof(loggerFactory));
    Guard.Argument(configuration).NotNull(nameof(configuration));

    _configuration = configuration;

    var appSettings = CreateAppSettings();
    _appSettings = Options.Create(appSettings);

    var logger = OLabLogger.CreateNew<OLabConfiguration>(loggerFactory);

    foreach (var item in configuration.AsEnumerable())
      logger.LogDebug($"{item.Key} -> {item.Value}");

  }

  public IConfiguration GetRawConfiguration() {  return _configuration; }

  //public IConfiguration GetConfiguration() { return _configuration; }
  public AppSettings GetAppSettings() { return _appSettings.Value; }

  private AppSettings CreateAppSettings()
  {
    var appSettings = new AppSettings();

    var properties = appSettings.GetType().GetProperties();
    foreach (var property in properties)
    {
      var value = GetValue<string>(property.Name);

      var prop = appSettings.GetType().GetProperty(property.Name, BindingFlags.Public | BindingFlags.Instance);
      if (null != prop && prop.CanWrite)
      {
        if (prop.PropertyType == typeof(string))
          prop.SetValue(appSettings, value, null);
        else if (prop.PropertyType == typeof(int))
          prop.SetValue(appSettings, Convert.ToInt32(value), null);
      }
    }

    return appSettings;
  }

#nullable enable
  public T? GetValue<T>(string section, string key, bool optional = false)
#nullable disable
  {
    var fullKey = $"{section}:{key}";
    var value = _configuration.GetValue<T>(fullKey);
    if (value == null && !optional)
      throw new ArgumentException($"cannot find configuration setting'{fullKey}'");
    return value;
  }

#nullable enable
  public T? GetValue<T>(string key, bool optional = false)
#nullable disable
  {
    var fullKey = $"{AppSettingPrefix}:{key}";
    var value = _configuration.GetValue<T>(fullKey);
    if (value == null && !optional)
      throw new ArgumentException($"cannot find configuration '{fullKey}'");
    return value;
  }

}
