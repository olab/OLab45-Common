﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OLab.Api.Utils;
using System;
using System.Reflection;

namespace OLab.Common.Utils;

public class Configuration
{
  public const string AppSettingPrefix = "AppSettings";

  private readonly IConfiguration _configuration;
  private IOptions<AppSettings> _appSettings;

  public Configuration(IConfiguration configuration)
  {
    _configuration = configuration;

    var appSettings = CreateAppSettings();
    _appSettings = Options.Create(appSettings);

  }

  public IConfiguration GetConfiguration() { return _configuration; }
  public IOptions<AppSettings> GetAppSettings() { return _appSettings; }

  private AppSettings CreateAppSettings()
  {
    var appSettings = new AppSettings();

    var properties = appSettings.GetType().GetProperties();
    foreach (var property in properties)
    {
      var value = GetValue<string>(property.Name);

      var prop = appSettings.GetType().GetProperty(property.Name, BindingFlags.Public | BindingFlags.Instance);
      if (null != prop && prop.CanWrite)
        prop.SetValue(appSettings, value, null);
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
      throw new ArgumentException($"cannot find '{fullKey}'");
    return value;
  }

#nullable enable
  public T? GetValue<T>(string key, bool optional = false)
#nullable disable
  {
    var fullKey = $"{AppSettingPrefix}:{key}";
    var value = _configuration.GetValue<T>(fullKey);
    if (value == null && !optional)
      throw new ArgumentException($"cannot find '{fullKey}'");
    return value;
  }

}
